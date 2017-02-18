using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Akka.Actor;

namespace WinTail
{
    public class TailActor : UntypedActor
    {
        private readonly string _filePath;
        private readonly IActorRef _reportActor;
        private readonly FileObserver _observer;
        private readonly Stream _fileStream;
        private readonly StreamReader _streamReader;

        public TailActor(IActorRef reportActor, string filePath)
        {
            _reportActor = reportActor;
            _filePath = filePath;

            var fullPath = Path.GetFullPath(filePath);
            _observer = new FileObserver(Self, fullPath);
            _observer.Start();

            _fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _streamReader = new StreamReader(_fileStream, Encoding.UTF8);

            var txt = _streamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, txt));
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                var text = _streamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reportActor.Tell(text);
                }
            }
            else if (message is FileError)
            {
                var fe = message as FileError;

            }
            else if (message is InitialRead)
            {

            }
        }

        public class FileWrite
        {
            public FileWrite(string fileName)
            {
                Filename = fileName;
            }

            public string Filename { get; private set; }
        }

        public class FileError
        {
            public FileError(string fileName, string reason)
            {
                Filename = fileName;
                Reason = reason;
            }

            public string Filename { get; private set; }
            public string Reason { get; private set; }
        }

        public class InitialRead
        {
            public InitialRead(string fileName, string text)
            {
                Filename = fileName;
                Text = text;
            }

            public string Filename { get; private set; }
            public string Text { get; private set; }
        }
    }
}
