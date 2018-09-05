/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License.  See LICENSE file in top-level directory.  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

using Upstream.System.Csv;
using Upstream.System.Records;
//using Upstream.System.Records.Csv;

namespace Upstream.System.Records.Csv
{
    class RecordCollectionEditorProgram
    {
        const string HelpText = 
         "rcd-edit tool version 20180905\n"
        +"\n"
        +"rcd-edit [OPTIONS] [InputFile]\n"
        +"\n"
        +"OPTIONS\n"
        +"    -J {F}  Editor script file name\n"
        +"    -o {F}  Output file name\n"
        +"    -W,-w   Read-write mode (output file is same as input file)\n"
        +"\n"
        +"Reads an input file into memory and then enters a command interpreter.\n"
        +"Writes the output only after all editor commands are completed.\n"
        +"If no output file is specified, then output will be written to STDOUT.\n"
        +"All prompts and results of interpreter output are sent to STDERR.\n"
        ;

        const string EditorHelpText = 
        "Record Editor\n"
        +"\n"
        +"Commands are CSV-formatted lists of variable (and optional) length.\n"
        +"The first value is the command name, followed by optional parameters.\n"
        +"\n"
        +"COMMANDS\n"
        +"\n"
        +"    help\n"
        +"        prints this help information.\n"
        +"    quit\n"
        +"        get out of here.\n"
        +"    print\n"
        +"        print the current list to the console\n"
        +"\n"
        ;

        const string EditorBannerText = 
        "Record Editor\n"
        +"Type 'help'<ENTER> to see help information.\n"
        ;

        internal static 
        int Main(string[] args)
        {
            int exitCode = 1;
            RecordCollectionEditorProgram exe = new RecordCollectionEditorProgram();

            exitCode = exe.Execute(args);

            return exitCode;
        }
                private int Execute(
             IEnumerable<string> args
            )
        {
            int exitCode = 0;
            TextReader ins = Console.In;
            TextWriter outs = Console.Out;
            TextWriter errs = Console.Error;
            bool showHelp = false;
            string errorMessage = null;

            //const string textEncodingDefault = "utf-8";
            const string csvSeparatorDefault = ",";
            CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

            string scriptTextEncodingNameIn = null;
            string scriptCsvSeparatorIn = null;
            string scriptFileNameIn = null;
            Encoding scriptTextEncodingIn = null;
            CsvEncoding scriptCsvEncodingIn = null;
            string scriptCultureNameIn = null;
            CultureInfo scriptCultureInfoIn = InvariantCulture;

            string scriptTextEncodingNameOut = null;
            string scriptCsvSeparatorOut = null;
            string scriptFileNameOut = null;
            Encoding scriptTextEncodingOut = null;
            CsvEncoding scriptCsvEncodingOut = null;
            string scriptCultureNameOut = null;
            CultureInfo scriptCultureInfoOut = InvariantCulture;

            string textEncodingNameIn = null;
            string csvSeparatorIn = null;
            string fileNameIn = null;
            Encoding textEncodingIn = null;
            CsvEncoding csvEncodingIn = null;
            string cultureNameIn = null;
            CultureInfo cultureInfoIn = InvariantCulture;
            
            string textEncodingNameOut = null;
            string csvSeparatorOut = null;
            string fileNameOut = null;
            Encoding textEncodingOut = null;
            CsvEncoding csvEncodingOut = null;
            string cultureNameOut = null;
            CultureInfo cultureInfoOut = InvariantCulture;

            TextWriter promptStreamOut = errs;
            bool shouldWriteToInput = false;

            IEnumerator<string> argEnumerator = args.GetEnumerator();
            while (argEnumerator.MoveNext())
            {
                string arg = argEnumerator.Current;
                if (null == arg)
                {
                }
                else if (ArgOptionEquals("-?", arg)
                    || ArgOptionEquals("--help", arg)
                    )
                {
                    showHelp = true;
                }
                else if (ArgOptionEquals("-w", arg)
                    || ArgOptionEquals("-W", arg)
                    || ArgOptionEquals("--read-write", arg)
                    )
                {
                    shouldWriteToInput = true;
                }
                else if (ArgOptionEquals("-J", arg)
                    || ArgOptionEquals("--script-in", arg)
                    || ArgOptionEquals("--script", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        scriptFileNameIn = arg;
                    }
                }
                else if (ArgOptionEquals("-L", arg)
                    || ArgOptionEquals("--locale-in", arg)
                    || ArgOptionEquals("--from-locale", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        cultureNameIn = arg;
                    }
                }
                else if (ArgOptionEquals("-l", arg)
                    || ArgOptionEquals("--locale-out", arg)
                    || ArgOptionEquals("--to-locale", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        cultureNameOut = arg;
                    }
                }
                else if (ArgOptionEquals("-E", arg)
                    || ArgOptionEquals("--encoding-in", arg)
                    || ArgOptionEquals("--from-encoding", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        textEncodingNameIn = arg;
                    }
                }
                else if (ArgOptionEquals("-e", arg)
                    || ArgOptionEquals("--encoding-out", arg)
                    || ArgOptionEquals("--to-encoding", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        textEncodingNameOut = arg;
                    }
                }
                else if (ArgOptionEquals("-S", arg)
                    || ArgOptionEquals("--separator-in", arg)
                    || ArgOptionEquals("--from-separator", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        csvSeparatorIn = arg;
                    }
                }
                else if (ArgOptionEquals("-s", arg)
                    || ArgOptionEquals("--separator-out", arg)
                    || ArgOptionEquals("--to-separator", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        csvSeparatorOut = arg;
                    }
                }
                else if (ArgOptionEquals("-o", arg)
                    || ArgOptionEquals("--output", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        fileNameOut = arg;
                    }
                }
                else if (!ArgIsOption(arg))
                {
                    if (null == fileNameIn)
                    {
                        fileNameIn = arg;
                    }
                }
            }

            // evaluate input script arguments:
            if (!String.IsNullOrEmpty(scriptCultureNameIn))
            {
                scriptCultureInfoIn = FindCultureInfo(scriptCultureNameIn);
                if (null == scriptCultureInfoIn)
                {
                    errorMessage = String.Format("Unknown culture name: '{0}'", scriptCultureNameIn);
                }
            }
            if (null != scriptTextEncodingNameIn)
            {
                scriptTextEncodingIn = FindTextEncoding(scriptTextEncodingNameIn);
                if (null == scriptTextEncodingIn)
                {
                    errorMessage = String.Format("Invalid input encoding: {0}", scriptTextEncodingNameIn);
                }
            }
            if (null != scriptCsvSeparatorIn)
            {
                scriptCsvSeparatorIn = ParseCsvSeparatorArg(scriptCsvSeparatorIn);
                scriptCsvEncodingIn = new CsvEncoding(scriptCsvSeparatorIn);
            }
            else
            {
                scriptCsvEncodingIn = new CsvEncoding(csvSeparatorDefault);
            }

            // evaluate script output arguments:
            if (!String.IsNullOrEmpty(scriptCultureNameOut))
            {
                scriptCultureInfoOut = FindCultureInfo(scriptCultureNameOut);
                if (null == scriptCultureInfoOut)
                {
                    errorMessage = String.Format("Unknown culture name: '{0}'", scriptCultureNameOut);
                }
            }
            if (null != scriptTextEncodingNameOut)
            {
                scriptTextEncodingOut = FindTextEncoding(scriptTextEncodingNameOut);
                if (null == scriptTextEncodingOut)
                {
                    errorMessage = String.Format("Invalid input encoding: {0}", scriptTextEncodingNameOut);
                }
            }
            if (null != scriptCsvSeparatorOut)
            {
                scriptCsvSeparatorOut = ParseCsvSeparatorArg(scriptCsvSeparatorOut);
                scriptCsvEncodingOut = new CsvEncoding(scriptCsvSeparatorOut);
            }
            else
            {
                scriptCsvEncodingOut = new CsvEncoding(csvSeparatorDefault);
            }

            // evaluate input stream arguments:
            if (!String.IsNullOrEmpty(cultureNameIn))
            {
                cultureInfoIn = FindCultureInfo(cultureNameIn);
                if (null == cultureInfoIn)
                {
                    errorMessage = String.Format("Unknown culture name: '{0}'", cultureNameIn);
                }
            }
            if (null != textEncodingNameIn)
            {
                textEncodingIn = FindTextEncoding(textEncodingNameIn);
                if (null == textEncodingIn)
                {
                    errorMessage = String.Format("Invalid input encoding: {0}", textEncodingNameIn);
                }
            }
            if (null != csvSeparatorIn)
            {
                csvSeparatorIn = ParseCsvSeparatorArg(csvSeparatorIn);
                csvEncodingIn = new CsvEncoding(csvSeparatorIn);
            }
            else
            {
                csvEncodingIn = new CsvEncoding(csvSeparatorDefault);
            }

            // evaluate output stream arguments:
            if (!String.IsNullOrEmpty(cultureNameOut))
            {
                cultureInfoOut = FindCultureInfo(cultureNameOut);
                if (null == cultureInfoOut)
                {
                    errorMessage = String.Format("Unknown culture name: '{0}'", cultureNameOut);
                }
            }
            if (null != textEncodingNameOut)
            {
                textEncodingOut = FindTextEncoding(textEncodingNameOut);
                if (null == textEncodingOut)
                {
                    errorMessage = String.Format("Invalid output encoding: {0}", textEncodingNameOut);
                }
            }
            if (null != csvSeparatorOut)
            {
                csvSeparatorOut = ParseCsvSeparatorArg(csvSeparatorOut);
                csvEncodingOut = new CsvEncoding(csvSeparatorOut);
            }
            else
            {
                csvEncodingOut = new CsvEncoding(csvSeparatorDefault);
            }

            if (shouldWriteToInput)
            {
                if (null == fileNameOut)
                {
                    fileNameOut = fileNameIn;
                }
                if (null == fileNameOut)
                {
                    errorMessage = "Cannot enter read-write mode; file name is missing\n";
                }
            }
            
            if (showHelp)
            {
                outs.Write(HelpText);
                outs.Flush();
            }
            else if (null != errorMessage)
            {
                errs.WriteLine(errorMessage);
                exitCode = 1;
            }
            else
            {
                FileStream scriptFileStreamIn = null;
                TextWriter scriptStreamOut = errs;
                FileStream fileStreamIn = null;
                FileStream fileStreamOut = null;

                try
                {
                    TextReader scriptStreamIn = ins;

                    if (!String.IsNullOrEmpty(scriptFileNameIn))
                    {
                        scriptFileStreamIn = OpenReadOnlyFileStream(scriptFileNameIn);
                        scriptStreamIn = CreateTextReader(scriptFileStreamIn, scriptTextEncodingIn);
                    }
                    CsvReader scriptCsvStreamIn = new CsvReader(scriptStreamIn, scriptCsvEncodingIn);

                    FieldSchemaSpecEncoding<object> fieldSpecEncoding = new FieldSchemaSpecEncoding<object>();
                    ArrayRecordList<object,IRecordFieldType<object>> recordList = new ArrayRecordList<object,IRecordFieldType<object>>();

                    // TODO: 20180905 refactor this into a function that reads a CSV file into an IRecordCollection
                    if (!String.IsNullOrEmpty(fileNameIn))
                    {
                        fileStreamIn = OpenReadOnlyFileStream(fileNameIn);
                        TextReader textStreamIn = CreateTextReader(fileStreamIn, textEncodingIn);
                        CsvReader csvIn = new CsvReader(textStreamIn, csvEncodingIn);

                        // read header row, and build a record schema in our record list
                        //BasicRecordSchema<IRecordFieldType<object>> recordSchemaIn = new BasicRecordSchema<IRecordFieldType<object>>();
                        //IList<string> fieldNameListIn = new List<string>();
                        if (csvIn.ReadRecord())
                        {
                            int startPosition = 0;
                            StringBuilder specParserBuffer = new StringBuilder();

                            while (csvIn.ReadValue())
                            {
                                string fieldSpec = csvIn.ValueText;
                                KeyValuePair<string,FieldSchemaSpecFieldType<object>> fieldInfo
                                    = fieldSpecEncoding.DecodeField(fieldSpec, startPosition, specParserBuffer);
                                string fieldName = fieldInfo.Key;
                                FieldSchemaSpecFieldType<object> fieldType = fieldInfo.Value;
                                //recordSchemaIn.AddField(fieldName, fieldType);
                                //fieldNameListIn.Add(fieldName);
                                recordList.AddField(
                                    fieldName
                                    ,fieldType
                                    );
                            }
                        }

                        IRecordSchemaAccessor<IRecordFieldType<object>> recordSchemaIn = recordList.RecordSchema;
                        IEnumerable<string> fieldNamesIn = recordSchemaIn.FieldNames;

                        // wrap the CSV stream with a string-valued record enumerator
                        IRecordEnumerator<string> csvInRecordEnumerator = new CsvRecordEnumerator(
                            csvIn
                            ,fieldNamesIn
                        );
                        // create a field value parser to parse string-valued records into general object-valued records
                        ParsingRecordAccessor<IRecordFieldType<object>> parsingRecordAdapter = new ParsingRecordAccessor<IRecordFieldType<object>>(
                             recordSchemaIn
                            ,cultureInfoIn
                        );
                        // wrap the string-valued record enumerator with a parsing adapter to make a stream of object-valued records
                        IRecordEnumerator<object> inRecordEnumerator = new RecordEnumeratorAdapter<object,IRecordAccessor<string>>(
                            parsingRecordAdapter
                            ,csvInRecordEnumerator
                        );
                        // get a collector from the list object that will receive the input records
                        IRecordCollectionBuilder<object> recordListBuilder = recordList.GetRecordCollectionBuilder();
                        // read all the records into the recordList
                        RecordIO.ReadInto<object>(
                             recordListBuilder
                            ,inRecordEnumerator
                        );
                        
                        // we are done with the input file,
                        // close file stream and set it to NULL so we don't try to close it twice
                        fileStreamIn.Close();
                        fileStreamIn = null;
                    }

                    // TODO 20180905 implement script interpreter
                    // EXECUTE editor read-eval-print loop here:

                    // script streams are CSV streams too:
                    CsvReader csvScriptIn = new CsvReader(scriptStreamIn, scriptCsvEncodingIn);
                    CsvWriter csvScriptOut = new CsvWriter(scriptStreamOut, scriptCsvEncodingOut);
                    Execute(
                        recordList
                        ,promptStreamOut
                        ,csvScriptOut
                        ,scriptCultureInfoOut
                        ,csvScriptIn
                        ,scriptCultureInfoIn
                        );

                    // Write the records from the list to the output stream:

                    IRecordSchemaAccessor<IRecordFieldType<object>> recordSchemaOut = recordList.RecordSchema;
                    IRecordEnumerator<object> recordEnumerator = recordList.GetRecordEnumerator();

                    TextWriter textStreamOut = outs;
                    if (!String.IsNullOrEmpty(fileNameOut))
                    {
                        fileStreamOut = OpenWriteOnlyFileStream(fileNameOut);
                        textStreamOut = CreateTextWriter(fileStreamOut, textEncodingOut);
                    }
                    CsvWriter csvOut = new CsvWriter(textStreamOut, csvEncodingOut);

                    // write header row to output CSV
                    // and build a list of the field names
                    IList<string> fieldNameListOut = new List<string>();
                    csvOut.WriteStartRecord();
                    foreach (KeyValuePair<string,IRecordFieldType<object>> fieldInfo in recordSchemaOut)
                    {
                        string fieldName = fieldInfo.Key;
                        IRecordFieldType<object> fieldType = fieldInfo.Value;
                        string fieldSpecString = fieldSpecEncoding.EncodeField(fieldName, fieldType);
                        fieldNameListOut.Add(fieldName);
                        csvOut.WriteValue(fieldSpecString);
                    }
                    csvOut.WriteEndRecord();

                    IRecordCollectionBuilder<string> csvOutRecordCollectionBuilder = new CsvRecordCollectionBuilder(
                        csvOut
                        ,fieldNameListOut
                    );
                    IRecordAccessorAdapter<string,IRecordAccessor<object>> outRecordAdapter = new PrintingRecordAccessor<object,IRecordFieldType<object>>(
                        recordSchemaOut
                        ,cultureInfoOut
                    );
                    IRecordEnumerator<string> outRecordEnumerator = new RecordEnumeratorAdapter<string,IRecordAccessor<object>>(
                         outRecordAdapter
                        ,recordEnumerator
                    );
                    RecordIO.ReadInto(
                         csvOutRecordCollectionBuilder
                        ,outRecordEnumerator
                    );

                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    errs.WriteLine(errorMessage);
                    exitCode = 1;
                }
                finally
                {
                    if (null != fileStreamOut)
                    {
                        fileStreamOut.Close();
                        fileStreamOut = null;
                    }
                    if (null != fileStreamIn)
                    {
                        fileStreamIn.Close();
                        fileStreamIn = null;
                    }
                    if (null != scriptFileStreamIn)
                    {
                        scriptFileStreamIn.Close();
                        scriptFileStreamIn = null;
                    }
                }
            }

            return exitCode;
        }

        /// <summary>
        /// Execute the record editor command interpreter
        /// to edit records in a list
        /// </summary>
        /// <param name="recordList">list/buffer where editing takes place</param>
        /// <param name="promptOut">stream to write user prompts</param>
        /// <param name="csvOut">stream to write user "print" output</param>
        /// <param name="csvIn">stream for reading user input commands</param>
        private void
        Execute(
            IRecordList<object,IRecordFieldType<object>> recordList
            ,TextWriter promptOut
            ,CsvWriter csvOut
            ,CultureInfo cultureInfoOut
            ,CsvReader csvIn
            ,CultureInfo cultureInfoIn
            )
        {
            string bannerText = EditorBannerText;
            string helpText = EditorHelpText;
            string promptText = ">> ";
            string cmdName;
            IRecordSchemaAccessor<IRecordFieldType<object>> recordSchema = recordList.RecordSchema;
            PrintingRecordAccessor<object,IRecordFieldType<object>> printingRecordAccessor = new PrintingRecordAccessor<object,IRecordFieldType<object>>(
                recordSchema
                ,cultureInfoOut
                );
            bool shouldQuit = false;
            Func<string,string,bool> CmdNamesAreEqual 
            = (string c1, string c2) =>
            {
                return (0 == String.Compare(c1,c2, StringComparison.Ordinal));
            }
            ;
            
            if (null != promptOut)
            {
                promptOut.Write(bannerText);
                promptOut.Write(promptText);
            }
            while (!shouldQuit
                && csvIn.ReadRecord()
                )
            {
                cmdName = null;
                if (csvIn.ReadValue())
                {
                    cmdName = csvIn.ValueText;    
                }

                if (null == cmdName)
                {
                    // continue;
                }
                else if (CmdNamesAreEqual("q", cmdName)
                    || CmdNamesAreEqual("quit", cmdName)
                    || CmdNamesAreEqual("exit", cmdName)
                    )
                {
                    shouldQuit = true;
                }
                else if (CmdNamesAreEqual("?", cmdName)
                    || CmdNamesAreEqual("help", cmdName)
                    )
                {
                    if (null != promptOut)
                    {
                        promptOut.Write(helpText);
                    }
                }
                else if (CmdNamesAreEqual("p", cmdName)
                    || CmdNamesAreEqual("print", cmdName)
                    )
                {
                    // TODO 20180905: print header
                    IRecordEnumerator<object> recordEnumerator = recordList.GetRecordEnumerator();
                    IRecordEnumerator<string> outRecordEnumerator = new RecordEnumeratorAdapter<string,IRecordAccessor<object>>(
                         printingRecordAccessor
                        ,recordEnumerator
                    );
                    IRecordCollectionBuilder<string> csvOutRecordCollectionBuilder = new CsvRecordCollectionBuilder(
                        csvOut
                        ,recordSchema.FieldNames
                    );
                    RecordIO.ReadInto(
                         csvOutRecordCollectionBuilder
                        ,outRecordEnumerator
                    );
                }
                else if (!String.IsNullOrWhiteSpace(cmdName))
                {
                    csvOut.WriteStartRecord();
                    csvOut.WriteValue("unknown_command");
                    csvOut.WriteValue(cmdName);
                    csvOut.WriteEndRecord();
                }

                if (null != promptOut
                    && !shouldQuit
                    )
                {
                    promptOut.Write(promptText);
                }
            }

        }


        private bool ArgIsOption(string arg)
        {
            if (null == arg)
            {
                return false;
            }

            return arg.StartsWith("-", StringComparison.Ordinal);
        }

        private bool ArgOptionEquals(string arg, string arg2)
        {
            return (0 == String.Compare(arg, arg2, StringComparison.Ordinal));
        }

        private string ParseCsvSeparatorArg(string arg)
        {
            string unitSeparator = arg;
            Func<string,string,bool> SeparatorSymbolEquals = 
            (string s1, string s2) =>
            {
                return (0 == String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase));
            };

            if (null == arg)
            {
            }
            else if (SeparatorSymbolEquals("tab", arg)
                || SeparatorSymbolEquals("\\t", arg)
                )
            {
                unitSeparator = "\t";
            }
            else if (SeparatorSymbolEquals("pipe", arg))
            {
                unitSeparator = "|";
            }
            else if (SeparatorSymbolEquals("space", arg)
                || SeparatorSymbolEquals("sp", arg)
                )
            {
                unitSeparator = " ";
            }

            return unitSeparator;
        }

        private CultureInfo FindCultureInfo(string cultureName)
        {
            CultureInfo cultureInfo = null;

            try
            {
                // User settings to locale will be ignored when using CultureInfo.GetCultureInfo()
                cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            }
            catch (CultureNotFoundException) // .NET 4.0 and later
            {
            }
            catch (ArgumentException) // .NET 3.5 and earlier
            {
            }

            return cultureInfo;
        }

        private Encoding FindTextEncoding(string encodingName)
        {
            if (null == encodingName)
            {
                return null;
            }

            Encoding foundEncoding = null;
            CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
            bool IgnoreCase = true;
            IEnumerable<EncodingInfo> encodingInfoCollection = Encoding.GetEncodings();
            IEnumerator<EncodingInfo> encodingInfoEnumerator = encodingInfoCollection.GetEnumerator();
            while (null == foundEncoding
                && encodingInfoEnumerator.MoveNext()
                )
            {
                EncodingInfo encodingInfo = encodingInfoEnumerator.Current;

                if (0 == String.Compare(encodingInfo.Name, encodingName, IgnoreCase, InvariantCulture))
                {
                    foundEncoding = encodingInfo.GetEncoding();
                }
            }

            return foundEncoding;
        }

        private FileStream
        OpenWriteOnlyFileStream(string fileName)
        {
            return new FileStream(
                            fileName
                            ,FileMode.Create
                            ,FileAccess.Write
                            ,FileShare.None
                            );

        }

        private TextWriter
        CreateTextWriter(
            Stream baseStream
            ,Encoding textEncoding
            )
        {
            TextWriter textStreamOut = null;

            if (null == textEncoding)
            {
                textStreamOut = new StreamWriter(baseStream);
            }
            else
            {
                textStreamOut = new StreamWriter(baseStream, textEncoding);
            }

            return textStreamOut;
        }

        private FileStream 
        OpenReadOnlyFileStream(string fileName)
        {
            return new FileStream(
                 fileName
                ,FileMode.Open
                ,FileAccess.Read
                ,FileShare.Read
                );
        }
        
        private TextReader
        CreateTextReader(
             Stream baseStream
            ,Encoding textEncoding
            )
        {
            TextReader textReader = null;

            if (null == textEncoding)
            {
                textReader = new StreamReader(baseStream);
            }
            else
            {
                textReader = new StreamReader(baseStream, textEncoding);
            }
            
            return textReader;
        }

    } // /class

}  // /namespace
