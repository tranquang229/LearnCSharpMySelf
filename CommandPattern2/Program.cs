// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using CommandPattern2;

DocumentInvoker instance = new DocumentInvoker();
instance.Write("The 1st text.");
instance.Undo();
instance.Read(); //EMPTY

instance.Redo();
instance.Read(); //The 1st text.


instance.Write("The 2nd text.");
instance.Write("The 3rd text.");
instance.Read(); //The 1st text.  The 2nd text.   The 3rd text.

instance.Undo();  //The 1st text.  The 2nd text.
instance.Undo();  //The 1st text.  
instance.Redo();  //The 1st text.    The 2nd text.
instance.Redo();  //The 1st text.    The 2nd text. The 3rd text.
instance.Redo();  //The 1st text.    The 2nd text. The 3rd text.
instance.Read();