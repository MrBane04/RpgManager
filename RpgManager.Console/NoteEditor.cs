namespace RpgManager.Console
{
    internal class NoteEditor
    {
        private string _content = string.Empty;
        private bool _isRunning = true;
        private bool _isSaved = false;

        private int _cursorPosition = 0;
        public NoteEditor(string content)
        {
            _content = content;
            _cursorPosition = _content.Length;
        }

        public string? Edit()
        {
            RefreshEditor();
            while ( _isRunning )
            {
                ConsoleKeyInfo keyInfo = System.Console.ReadKey(true);
                

                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (_cursorPosition > 0)
                        {
                            _cursorPosition--;
                            RefreshEditor();
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (_cursorPosition < _content.Length)
                        {
                            _cursorPosition++;
                            RefreshEditor();
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        MoveVertical(-1);
                        RefreshEditor();
                        break;
                    case ConsoleKey.DownArrow:
                        MoveVertical(1);
                        RefreshEditor();
                        break;
                    case ConsoleKey.Escape:
                        _isRunning = false;
                        _isSaved = false;
                        break;
                    case ConsoleKey.F2:
                        
                            _isRunning = false;
                            _isSaved = true;
                        
                        break;
                    case ConsoleKey.Backspace:
                        if (_cursorPosition >0 )
                        {
                            _content = _content.Remove(_cursorPosition -1,1);
                            _cursorPosition --;
                            RefreshEditor();
                        }
                        break;
                    case ConsoleKey.Enter:
                        _content = _content.Insert(_cursorPosition, Environment.NewLine);
                        _cursorPosition += Environment.NewLine.Length;

                        RefreshEditor();
                        break;
                    default:
                        if(!char.IsControl(keyInfo.KeyChar))
                        {
                            _content = _content.Insert(_cursorPosition, keyInfo.KeyChar.ToString());
                            _cursorPosition++;
                            RefreshEditor();
                        }
                        
                        break;
                }
            }
            return _isSaved ? _content : null;
        }

        private void RefreshEditor()
        {
            System.Console.Clear();
            System.Console.WriteLine("== Edytor Notatki ==");
            System.Console.WriteLine("F2 - Zapisz i wyjdź | Esc - Anuluj");
            System.Console.WriteLine("----------------------------------------");
            
            int startRow = 3;
            int consoleWidth = System.Console.WindowWidth;
            System.Console.Write(_content);
            int cursorRow = startRow;
            int cursorColumn = 0;

            for(int i =0;i< _cursorPosition; i++)
            {
                if (_content[i] == '\n')
                {
                    cursorRow++;
                    cursorColumn = 0;
                }
                else if (_content[i] != '\r')
                {
                    cursorColumn++;
                    if(cursorColumn >= consoleWidth)
                    {
                        cursorColumn = 0;
                        cursorRow++;
                    }
                }
            }
            //cursorColumn = Math.Min(cursorColumn, System.Console.BufferWidth - 1);
            //cursorRow = Math.Min(cursorRow, System.Console.BufferHeight - 1);
            System.Console.SetCursorPosition(cursorColumn, cursorRow);
        }


        private void MoveVertical(int direction)
        {
            int width = Math.Max(1, System.Console.WindowWidth);

            List<(int Start, int End)> visualLines = new();

            int lineStart = 0;

            while (lineStart <= _content.Length)
            {
                int lineEnd = _content.IndexOf('\n', lineStart);

                if (lineEnd == -1)
                {
                    lineEnd = _content.Length;
                }

                int contentEnd = lineEnd;

                if (contentEnd > lineStart && _content[contentEnd - 1] == '\r')
                {
                    contentEnd--;
                }

                int segmentStart = lineStart;

                while (segmentStart < contentEnd)
                {
                    int segmentEnd = Math.Min(segmentStart + width, contentEnd);

                    visualLines.Add((segmentStart, segmentEnd));
                    segmentStart = segmentEnd;
                }

                if (segmentStart == lineStart)
                {
                    visualLines.Add((lineStart, contentEnd));
                }

                if (lineEnd == _content.Length)
                {
                    break;
                }

                lineStart = lineEnd + 1;
            }

            int currentLineIndex = visualLines.FindIndex(
                line => _cursorPosition >= line.Start &&
                        _cursorPosition < line.End);

            if (currentLineIndex == -1)
            {
                currentLineIndex = visualLines.FindLastIndex(
                    line => _cursorPosition == line.End);
            }

            int targetLineIndex = currentLineIndex + direction;

            if (currentLineIndex == -1 ||
                targetLineIndex < 0 ||
                targetLineIndex >= visualLines.Count)
            {
                return;
            }

            var currentLine = visualLines[currentLineIndex];
            var targetLine = visualLines[targetLineIndex];

            int column = _cursorPosition - currentLine.Start;

            _cursorPosition = Math.Min(
                targetLine.Start + column,
                targetLine.End);
        }


    }
}