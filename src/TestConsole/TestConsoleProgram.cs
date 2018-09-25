/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Upstream.System
{
    /// <summary>
    /// Main entry point class for Upstream.System library test console program
    /// </summary>
    class TestConsoleProgram
    {
        const string _helpText = 
         "Upstream System Library Test tool version 20170308:20180325\n"
        +"\n"
        +"TestConsole [OPTIONS] [CommandName [CommandOptions]]\n"
        +"\n"
        +"OPTIONS\n"
        +"    --help\n"
        +"\n"
        +"COMMANDS\n"
        +"{COMMAND_LIST}"
        +"\n"
        +"When no command is specified, the test console will enter an interactive mode.\n"
        +"Individual commands can be executed by name in interactive mode.\n"
        +"Additional, interactive commands are:\n"
        +"    cd\n"
        +"    dir\n"
        +"    exit\n"
        +"    help\n"
        ;
        
        static int Main(string[] args)
        {
            int exitCode = 1;
            string CommandListHelpTextToken = "{COMMAND_LIST}";
            string CommandNameHelpTextFormatString = "    {0}\n";
            string cmdOptionPrefix = "-";
            string cmdName = null;
            string helpText;
            bool showHelp = false;
            bool shouldInteract = true;
            string interationPrompt = "[testconsole]? ";
            TextReader ins = Console.In;
            TextWriter outs = Console.Out;
            TextWriter errs = Console.Error;
            int argPosition;
            int cmdArgPosition = -1;
            string arg;
            StringComparer cmdOptionComparer = StringComparer.Ordinal;
            StringComparer cmdNameComparer = StringComparer.OrdinalIgnoreCase;
            Func<string,string,bool> CmdOptionsAreEqual = 
            (string s1, string s2) =>
            {
                return (0 == cmdOptionComparer.Compare(s1, s2));
            };
            Func<string,string,bool> CmdNamesAreEqual =
            (string s1, string s2) =>
            {
                return (0 == cmdNameComparer.Compare(s1, s2));
            };
            IDictionary<string,Func<string[],int>> cmdDictionary = new Dictionary<string,Func<string[],int>>(cmdNameComparer);
            
            // register available test program commands in a dictionary
            cmdDictionary.Add("csv-translate", Csv.CsvTranslateProgram.Main);
            cmdDictionary.Add("field_schema_spec-print", Records.FieldSchemaSpecPrintProgram.Main);
            cmdDictionary.Add("rcd-csv-translate", Records.Csv.CsvRecordTranslateProgram.Main);
            cmdDictionary.Add("rcd-csv-select", Records.Csv.CsvRecordSelectionProgram.Main);
            cmdDictionary.Add("rcd-editor", Records.Csv.RecordCollectionEditorProgram.Main);

            // insert command names into the help text
            StringBuilder cmdListBuffer = new StringBuilder();
            foreach (string cmdKey in cmdDictionary.Keys)
            {
                cmdListBuffer.AppendFormat(
                    CommandNameHelpTextFormatString
                    ,cmdKey
                    );
            }
            helpText = _helpText;
            helpText = helpText.Replace(CommandListHelpTextToken, cmdListBuffer.ToString());


            // parse test console options
            argPosition = 0;
            while (argPosition < args.Length
                && null == cmdName
                )
            {
                arg = args[argPosition];
                if (null == arg)
                {
                    //continue;
                }
                else if (CmdOptionsAreEqual("-?", arg)
                    || CmdOptionsAreEqual("--help", arg)
                    )
                {
                    showHelp = true;
                }
                else if (!arg.StartsWith(cmdOptionPrefix))
                {
                    cmdArgPosition = argPosition;
                    cmdName = arg;
                    shouldInteract = false;  // should quit immediately after executing this command
                }

                argPosition += 1;
            }

            if (null != cmdName
                && CmdNamesAreEqual("help", cmdName)
                )
            {
                showHelp = true;
            }

            if (showHelp)
            {
                exitCode = 0;
                outs.Write(helpText);
            }
            else
            {
                string[] cmdArgs = cmdArgs = CreateShiftedArray(args, cmdArgPosition+1);

                do
                {
                    if (String.IsNullOrWhiteSpace(cmdName))
                    {
                        // do nothing
                    }
                    else if (CmdNamesAreEqual("quit", cmdName)
                        || CmdNamesAreEqual("exit", cmdName)
                        )
                    {
                        shouldInteract = false;
                    }
                    else if (CmdNamesAreEqual("help", cmdName))
                    {
                        // [20170308 [db] this isn't really the best help to show, but better than nothing for now]
                        outs.Write(helpText);
                    }
                    else if (CmdNamesAreEqual("pwd", cmdName))
                    {
                        string dirPath = Directory.GetCurrentDirectory();
                        outs.WriteLine(dirPath);
                    }
                    else if (CmdNamesAreEqual("cd", cmdName))
                    {
                        if (0 < cmdArgs.Length)
                        {
                            string dirPath = cmdArgs[0];
                            if (Directory.Exists(dirPath))
                            {
                                Directory.SetCurrentDirectory(dirPath);
                            }
                            else
                            {
                                errs.WriteLine("Directory does not exist");
                            }
                        }
                        else
                        {
                            string dirPath = Directory.GetCurrentDirectory();
                            outs.WriteLine(dirPath);
                        }
                    }
                    else if (CmdNamesAreEqual("dir", cmdName))
                    {
                        string dirPath = ".";
                        if (0 < cmdArgs.Length)
                        {
                            dirPath = cmdArgs[0];
                        }
                        IEnumerable<string> fsEntryNames = Directory.GetFileSystemEntries(dirPath);
                        foreach (string fsEntryName in fsEntryNames)
                        {
                            outs.WriteLine(fsEntryName);
                        }
                    }
                    else
                    // look in the cmdDictionary for a sub-program to execute
                    {
                        Func<string[],int> cmdMainFunc = null;
                        if (cmdDictionary.TryGetValue(cmdName, out cmdMainFunc))
                        {
                            exitCode = cmdMainFunc.Invoke(cmdArgs);
                        }
                    }

                    if (shouldInteract)
                    {
                        outs.Write(interationPrompt);
                        string commandLineString = ins.ReadLine();
                        if (null == commandLineString)
                        {
                            // end-of-file
                            shouldInteract = false;
                        }
                        else
                        {
                            cmdArgs = ParseCommandline(commandLineString);
                            if (0 < cmdArgs.Length)
                            {
                                cmdArgPosition = 0;
                                cmdName = cmdArgs[cmdArgPosition];
                                cmdArgs = CreateShiftedArray(cmdArgs, cmdArgPosition+1);
                            }
                        }
                    }
                } while (shouldInteract);
            }

            return exitCode;
        }

        private static string[]
        CreateShiftedArray(
            string[] inArray
            ,int startPosition
            )
        {
            string[] outArray = null;

            if (null != inArray)
            {
                if (0 <= startPosition
                    && startPosition < inArray.Length
                    )
                {
                    outArray = new string[inArray.Length-startPosition];
                    int outPosition;
                    for (outPosition = 0; outPosition < outArray.Length; outPosition++)
                    {
                        outArray[outPosition] = inArray[startPosition+outPosition];
                    }
                }
                else
                {
                    outArray = new string[0];
                }
            }

            return outArray;
        }

        /// <summary>
        /// Parse a commandline into an array of commandline arguments.
        /// Commandline syntax is similar to the Windows command shell.
        /// </summary>
        /// <param name="commandLineString"></param>
        /// <returns></returns>
        private static string[]
        ParseCommandline(
            string commandLineString
            )
        {
            CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
            char nullChar = '\0';
            string quoteSymbols = "\"'";
            //string escapeSymbol = "\\";
            string whitespaceSymbols = " \t";
            Func<string,int,string,bool> SubstringEquals =
            (string s, int i, string s2) =>
            {
                return (0 == String.Compare(s, i, s2, 0, s2.Length, InvariantCulture, CompareOptions.Ordinal));
            };
            Func<char,string,char,char>  FilterChar =
            (char c, string filterChars, char defaultChar) =>
            {
                char outCh = defaultChar;
                int i = filterChars.IndexOf(c);
                if (0 <= i)
                {
                    outCh = filterChars[i];
                }
                return outCh;
            };
            ICollection<string> argCollection = new List<string>();
            StringBuilder buffer = new StringBuilder();
            string arg;
            int charPosition;
            char ch;
            char quoteChar;
            char wsChar;
            StringBuilder argBuffer;

            arg = null;
            // argBuffer will be nonnull if an arg is quoted or if we read non-whitespace
            argBuffer = null;
            quoteChar = nullChar;
            charPosition = 0;
            while (charPosition < commandLineString.Length)
            {
                ch = commandLineString[charPosition];
                wsChar = FilterChar(ch, whitespaceSymbols, nullChar);
                if (nullChar != quoteChar)
                {
                    if (quoteChar == ch)
                    {
                        // end quote
                        quoteChar = nullChar;
                    }
                    else if (null != argBuffer)
                    {
                        argBuffer.Append(ch);
                    }
                }
                else if (nullChar != wsChar)
                {
                    if (null != argBuffer)
                    {
                        arg = argBuffer.ToString();
                    }
                }
                else
                {
                    argBuffer = buffer;
                    quoteChar = FilterChar(ch, quoteSymbols, nullChar);
                    if (nullChar == quoteChar)
                    {
                        // not in a quote, not whitespace
                        argBuffer.Append(ch);
                    }
                }

                charPosition += 1;
                if (charPosition >= commandLineString.Length)
                {
                    // push the last argument
                    if (null != argBuffer)
                    {
                        arg = argBuffer.ToString();
                    }
                }

                if (null != arg)
                {
                    buffer.Clear();
                    argBuffer = null;
                    argCollection.Add(arg);
                    arg = null;
                }
            }

            string[] argArray = new String[argCollection.Count];
            argCollection.CopyTo(argArray, 0);

            return argArray;
        }

    } // /class

}  // /namespace

