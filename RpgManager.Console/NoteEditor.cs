namespace RpgManager.Console
{
    internal class NoteEditor
    {
        private string _content = string.Empty;
        private bool _isRunning = true;
        private bool _isSaved = false;
        public NoteEditor(string content)
        {
            _content = content;
        }

        public string? Edit()
        {
            System.Console.Clear();
            System.Console.WriteLine("== Edytor Notatki ==");
            System.Console.WriteLine("Ctrl + S - Zapisz i wyjdź | Esc - Anuluj");
            System.Console.WriteLine("----------------------------------------");
            System.Console.Write(_content);
            while ( _isRunning )
            {
                ConsoleKeyInfo keyInfo = System.Console.ReadKey(true);

                switch(keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        _isRunning = false;
                        _isSaved = false;
                        break;
                    case ConsoleKey.S:
                        if(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            _isRunning = false;
                            _isSaved = true;
                        }
                        break;
                    default:
                        if(!char.IsControl(keyInfo.KeyChar))
                        {
                            _content += keyInfo.KeyChar;
                            System.Console.Write(keyInfo.KeyChar);
                        }
                        else if(keyInfo.Key == ConsoleKey.Enter)
                        {
                            _content += Environment.NewLine;
                            System.Console.WriteLine();
                        }
                        break;
                }
            }
            return _isSaved ? _content : null;
        }
    }
}