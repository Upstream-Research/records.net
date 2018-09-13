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
        +"    -a {S}  Comma-separated list of fields to append to records\n"
        +"    -J {F}  Editor script file name\n"
        +"    -o {F}  Output file name\n"
        +"    -W,-w   Read-write mode (output file is same as input file)\n"
        +"\n"
        +"Reads an input file into memory and then enters a command interpreter.\n"
        +"Writes the output only after all editor commands are completed.\n"
        +"If no input file is specified, then a new recordset will be created in memory.\n"
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
        +"        print the record to the console\n"
        +"    header\n"
        +"        print the field schema header\n"
        +"    goto,<N>\n"
        +"        go to record <N>\n"
        +"    up\n"
        +"        move 'up' to previous record\n"
        +"    down\n"
        +"        move 'down' to next record\n"
        +"    left\n"
        +"        move 'left' to the previous field\n"
        +"    right\n"
        +"        move 'right' to the next field\n"
        +"    replace,<V>\n"
        +"        replace the value of the current field with <V>\n"
        +"\n"
        ;

        const string EditorBannerText = 
        "Record Editor\n"
        +"Type 'help'<ENTER> to see help information.\n"
        ;

        const string EditorPromptTemplateString = "[{RecordPosition},{FieldPosition},{FieldName}]>> ";

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

            string appendFieldSchemaSpec = null;
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
                else if (ArgOptionEquals("-a", arg)
                    || ArgOptionEquals("--append-fields", arg)
                    )
                {
                    if (argEnumerator.MoveNext())
                    {
                        arg = argEnumerator.Current;
                        appendFieldSchemaSpec = arg;
                    }
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
                        BasicRecordSchema<IRecordFieldType<object>> recordSchemaIn = new BasicRecordSchema<IRecordFieldType<object>>();

                        // read header row, and build a record schema in our record list
                        if (csvIn.ReadRecord())
                        {
                            int startPosition = 0;
                            StringBuilder specParserBuffer = new StringBuilder();

                            while (csvIn.ReadValue())
                            {
                                string fieldSpec = csvIn.ValueText;
                                IFieldNameValuePair<IRecordFieldType<object>> fieldInfo
                                    = fieldSpecEncoding.DecodeField(fieldSpec, startPosition, specParserBuffer);
                                string fieldName = fieldInfo.Name;
                                IRecordFieldType<object> fieldType = fieldInfo.Value;
                                recordSchemaIn.AddField(
                                     fieldName
                                    ,fieldType
                                    );
                                recordList.AddField(
                                     fieldName
                                    ,fieldType
                                    );
                            }
                        }

                        // append the additional fields specified on the commandline only to the recordList
                        if (!String.IsNullOrEmpty(appendFieldSchemaSpec))
                        {
                            IEnumerable<IFieldNameValuePair<IRecordFieldType<object>>> fieldInfoEnumeration
                            = fieldSpecEncoding.DecodeEnumerable(appendFieldSchemaSpec);
                            foreach(IFieldNameValuePair<IRecordFieldType<object>> fieldInfo in fieldInfoEnumeration)
                            {
                                string fieldName = fieldInfo.Name;
                                IRecordFieldType<object> fieldType = fieldInfo.Value;
                                recordList.AddField(
                                     fieldName
                                    ,fieldType
                                    );
                            }
                        }

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

                    // script streams are CSV streams too:
                    CsvReader csvScriptIn = new CsvReader(scriptStreamIn, scriptCsvEncodingIn);
                    CsvWriter csvScriptOut = new CsvWriter(scriptStreamOut, scriptCsvEncodingOut);

                    // EXECUTE the read-eval-print loop:
                    Execute(
                        recordList
                        ,promptStreamOut
                        ,csvScriptOut
                        ,scriptCultureInfoOut
                        ,csvScriptIn
                        ,scriptCultureInfoIn
                        ,fieldSpecEncoding
                        );

                    // Write the records from the list to the output stream:

                    IRecordSchemaViewer<IRecordFieldType<object>> recordSchemaOut = recordList.RecordSchema;
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
                    foreach (IFieldNameValuePair<IRecordFieldType<object>> fieldInfo in recordSchemaOut)
                    {
                        string fieldName = fieldInfo.Name;
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
            ,FieldSchemaSpecEncoding<object> fieldSpecEncoding
            )
        {
            string bannerText = EditorBannerText;
            string helpText = EditorHelpText;
            string promptTemplateString = EditorPromptTemplateString;
            string promptString;
            string cmdName;
            int currentRecordPosition = 0;
            int currentFieldPosition = 0;
            string currentFieldName = String.Empty;
            IRecordSchemaViewer<IRecordFieldType<object>> recordSchema = recordList.RecordSchema;
            PrintingRecordAccessor<object,IRecordFieldType<object>> printingRecordAccessor = new PrintingRecordAccessor<object,IRecordFieldType<object>>(
                recordSchema
                ,cultureInfoOut
                );
            IRecordListVisitor<object> recordVisitor;
            IRecordAccessor<object> currentRecord = null;
            bool shouldQuit = false;
            Func<string,string,bool> CmdNamesAreEqual 
            = (string c1, string c2) =>
            {
                return (0 == String.Compare(c1,c2, StringComparison.Ordinal));
            }
            ;
            Action<CsvWriter,IRecordAccessor<string>> PrintRecordInRow
            = (CsvWriter csvWriter, IRecordAccessor<string> strRecord)
            =>
            {
                csvWriter.WriteStartRecord();
                foreach(IFieldNameValuePair<string> field in strRecord)
                {
                    csvWriter.WriteValue(field.Value);
                }
                csvWriter.WriteEndRecord();
            }
            ;
            Action<CsvWriter,IRecordAccessor<string>> PrintRecordInColumn
            = (CsvWriter csvWriter, IRecordAccessor<string> strRecord)
            =>
            {
                foreach(IFieldNameValuePair<string> field in strRecord)
                {
                    csvWriter.WriteStartRecord();
                    csvWriter.WriteValue(field.Name);
                    csvWriter.WriteValue(field.Value);
                    csvWriter.WriteEndRecord();
                }
            }
            ;
            Action<CsvWriter,IRecordAccessor<string>,string> PrintField
            = (CsvWriter csvWriter, IRecordAccessor<string> strRecord, string fieldName)
            =>
            {
                string fieldValueString = strRecord[fieldName];
                csvWriter.WriteStartRecord();
                csvWriter.WriteValue(fieldName);
                csvWriter.WriteValue(fieldValueString);
                csvWriter.WriteEndRecord();
            }
            ;
            Func<CsvReader,int,int> ParseInt32
            = (CsvReader csvReader, int defaultValue)
            =>
            {
                int parsedValue = defaultValue;
                if (csvReader.ReadValue())
                {
                    string valueString = csvReader.ValueText;
                    if (!Int32.TryParse(valueString, NumberStyles.Integer, cultureInfoIn, out parsedValue))
                    {
                        parsedValue = defaultValue;
                    }
                }

                return parsedValue;
            }
            ;
            Func<int,int,string,string> FormatPrompt
            = (int recordNum, int fieldNum, string fieldName)
            =>
            {
                return promptTemplateString
                    .Replace("{FieldName}", fieldName)
                    .Replace("{FieldPosition}", fieldNum.ToString(cultureInfoOut))
                    .Replace("{RecordPosition}", recordNum.ToString(cultureInfoOut))
                    ;
            }
            ;
            
            recordVisitor = recordList.GetRecordListVisitor();
            if (recordVisitor.MoveNext())
            {
                currentRecord = recordVisitor.Current;
                printingRecordAccessor.AttachTo(currentRecord);
            }
            if (currentFieldPosition < recordSchema.GetFieldCount())
            {
                currentFieldName = recordSchema.FieldNameAt(currentFieldPosition);
            }

            if (null != promptOut)
            {
                promptOut.Write(bannerText);
                promptString = FormatPrompt(currentRecordPosition, currentFieldPosition, currentFieldName); 
                promptOut.Write(promptString);
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
                else if (CmdNamesAreEqual("g", cmdName)
                    || CmdNamesAreEqual("goto", cmdName)
                    )
                {
                    int newPosition = ParseInt32(csvIn, currentRecordPosition);
                    if (newPosition < recordList.Count)
                    {
                        if (recordVisitor.MoveTo(newPosition))
                        {
                            currentRecordPosition = newPosition;
                            currentRecord = recordVisitor.Current;
                            printingRecordAccessor.AttachTo(currentRecord);
                            PrintRecordInRow(csvOut, printingRecordAccessor);
                        }
                    }
                }
                else if (CmdNamesAreEqual("j", cmdName)
                    || CmdNamesAreEqual("down", cmdName)
                    )
                {
                    int defaultOffset = 1;
                    int offset = ParseInt32(csvIn, defaultOffset);
                    int newPosition = currentRecordPosition + offset;
                    if (newPosition < recordList.Count)
                    {
                        if (recordVisitor.MoveTo(newPosition))
                        {
                            currentRecordPosition = newPosition;
                            currentRecord = recordVisitor.Current;
                            printingRecordAccessor.AttachTo(currentRecord);
                            PrintRecordInRow(csvOut, printingRecordAccessor);
                        }
                    }
                }
                else if (CmdNamesAreEqual("k", cmdName)
                    || CmdNamesAreEqual("up", cmdName)
                    )
                {
                    int defaultOffset = 1;
                    int offset = ParseInt32(csvIn, defaultOffset);
                    if (offset <= currentRecordPosition)
                    {
                        int newPosition = currentRecordPosition - offset;
                        if (recordVisitor.MoveTo(newPosition))
                        {
                            currentRecordPosition = newPosition;
                            currentRecord = recordVisitor.Current;
                            printingRecordAccessor.AttachTo(currentRecord);
                            PrintRecordInRow(csvOut, printingRecordAccessor);
                        }
                    }
                }
                else if (CmdNamesAreEqual("h", cmdName)
                    || CmdNamesAreEqual("left", cmdName)
                    )
                {
                    int defaultOffset = 1;
                    int offset = ParseInt32(csvIn, defaultOffset);
                    if (offset <= currentFieldPosition)
                    {
                        currentFieldPosition -= offset;
                        currentFieldName = recordSchema.FieldNameAt(currentFieldPosition);
                        if (null != currentRecord)
                        {
                            printingRecordAccessor.AttachTo(currentRecord);
                            PrintField(csvOut, printingRecordAccessor, currentFieldName);
                        }
                    }
                }
                else if (CmdNamesAreEqual("l", cmdName)
                    || CmdNamesAreEqual("right", cmdName)
                    )
                {
                    int defaultOffset = 1;
                    int offset = ParseInt32(csvIn, defaultOffset);
                    if (currentFieldPosition + offset < recordList.FieldCount)
                    {
                        currentFieldPosition += offset;
                        currentFieldName = recordSchema.FieldNameAt(currentFieldPosition);
                        if (null != currentRecord)
                        {
                            printingRecordAccessor.AttachTo(currentRecord);
                            PrintField(csvOut, printingRecordAccessor, currentFieldName);
                        }
                    }
                }
                else if (CmdNamesAreEqual("p", cmdName)
                    || CmdNamesAreEqual("print", cmdName)
                    )
                {
                    printingRecordAccessor.AttachTo(currentRecord);
                    PrintRecordInRow(csvOut, printingRecordAccessor);
                }
                else if (CmdNamesAreEqual("H", cmdName)
                    || CmdNamesAreEqual("header", cmdName)
                    )
                {
                    csvOut.WriteStartRecord();
                    foreach (IFieldNameValuePair<IRecordFieldType<object>> fieldInfo in recordList.RecordSchema)
                    {
                        string fieldName = fieldInfo.Name;
                        IRecordFieldType<object> fieldType = fieldInfo.Value;
                        string fieldSpecString = fieldSpecEncoding.EncodeField(fieldName, fieldType);
                        csvOut.WriteValue(fieldSpecString);
                    }
                    csvOut.WriteEndRecord();
                }
                else if (CmdNamesAreEqual("x", cmdName)
                    || CmdNamesAreEqual("set_null", cmdName)
                    )
                {
                    if (null != currentRecord
                        && 0 <= currentFieldPosition
                        && recordList.FieldCount > currentFieldPosition
                        )
                    {
                        currentRecord[currentFieldPosition] = null;
                    }
                }
                else if (CmdNamesAreEqual("r", cmdName)
                    || CmdNamesAreEqual("replace", cmdName)
                    )
                {
                    string fieldValueString = null;

                    if (csvIn.ReadValue())
                    {
                        fieldValueString = csvIn.ValueText;
                    }
                    if (null != currentRecord
                        && 0 <= currentFieldPosition
                        && recordList.FieldCount > currentFieldPosition
                        )
                    {
                        IRecordFieldType<object> fieldType = recordSchema[currentFieldPosition];
                        Type dataType = fieldType.DataType;
                        BasicFieldValueStringRepresentation<object> fieldFormatter = new BasicFieldValueStringRepresentation<object>(dataType, cultureInfoIn);
                        object fieldValue;
                        fieldFormatter.TryParse(fieldValueString, out fieldValue);
                        currentRecord[currentFieldPosition] = fieldValue;
                        printingRecordAccessor.AttachTo(currentRecord);
                        PrintField(csvOut, printingRecordAccessor, currentFieldName);
                    }
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
                    promptString = FormatPrompt(
                        currentRecordPosition
                        ,currentFieldPosition
                        ,currentFieldName
                        );
                    promptOut.Write(promptString);
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
