/*  Copyright (c) 2017 Upstream Research, Inc.  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Upstream.System.Csv;

namespace Upstream.System.Records.Csv
{
    /// <summary>
    /// Very basic CSV Record processing tool,
    /// used for testing CSV Record support classes.
    /// Converts CSV encodings and charset encodings
    /// </summary>
    class CsvRecordTranslateProgram
    {
        const string HelpText = 
         "RCD-CSV-TRANSLATE tool version 20180324\n"
        +"\n"
        +"rcd-csv-translate [OPTIONS] [InputFile]\n"
        +"\n"
        +"OPTIONS\n"
        +"    -E {E}  Input file text encoding (e.g. 'utf-8', 'windows-1252')\n"
        +"    -e {E}  Output file text encoding (e.g. 'utf-8', 'windows-1252')\n"
        +"    -o {F}  Output file name\n"
        +"    -S {S}  Input file field delimiter\n"
        +"    -s {S}  Output file field delimiter\n"
        +"    -x      Parse and rewrite field values using CSV header row type spec\n"
        ;

        internal static 
        int Main(string[] args)
        {
            int exitCode = 1;
            CsvRecordTranslateProgram exe = new CsvRecordTranslateProgram();

            exitCode = exe.Execute(args);

            return exitCode;
        }

        private int Execute(
             IEnumerable<string> args
            )
        {
            int exitCode = 0;
            //const string textEncodingDefault = "utf-8";
            const string csvSeparatorDefault = ",";
            string textEncodingNameIn = null;
            string textEncodingNameOut = null;
            string csvSeparatorIn = null;
            string csvSeparatorOut = null;
            string fileNameIn = null;
            string fileNameOut = null;
            bool showHelp = false;
            Encoding textEncodingIn = null;
            Encoding textEncodingOut = null;
            CsvEncoding csvEncodingIn = null;
            CsvEncoding csvEncodingOut = null;
            bool shouldParseFieldValues = false;
            TextReader ins = Console.In;
            TextWriter outs = Console.Out;
            TextWriter errs = Console.Error;
            string errorMessage = null;

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
                else if (ArgOptionEquals("-x", arg)
                    || ArgOptionEquals("--translate-types", arg)
                )
                {
                    shouldParseFieldValues = true;
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

            if (null != textEncodingNameIn)
            {
                textEncodingIn = FindTextEncoding(textEncodingNameIn);
                if (null == textEncodingIn)
                {
                    errorMessage = String.Format("Invalid input encoding: {0}", textEncodingNameIn);
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
            if (null != csvSeparatorIn)
            {
                csvSeparatorIn = ParseCsvSeparatorArg(csvSeparatorIn);
                csvEncodingIn = new CsvEncoding(csvSeparatorIn);
            }
            else
            {
                csvEncodingIn = new CsvEncoding(csvSeparatorDefault);
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

            if (showHelp)
            {
                outs.Write(HelpText);
                outs.Flush();
                exitCode = -1;
            }
            else if (null != errorMessage)
            {
                errs.WriteLine(errorMessage);
                exitCode = 1;
            }
            else
            {
                Stream fileStreamIn = null;
                Stream fileStreamOut = null;

                try
                {
                    if (!String.IsNullOrEmpty(fileNameIn))
                    {
                        fileStreamIn = new FileStream(
                            fileNameIn
                            ,FileMode.Open
                            ,FileAccess.Read
                            ,FileShare.Read
                            );
                        if (null == textEncodingIn)
                        {
                            ins = new StreamReader(fileStreamIn);
                        }
                        else
                        {
                            ins = new StreamReader(fileStreamIn, textEncodingIn);
                        }
                    }
                    if (!String.IsNullOrEmpty(fileNameOut))
                    {
                        fileStreamOut = new FileStream(
                            fileNameOut
                            ,FileMode.Create
                            ,FileAccess.Write
                            ,FileShare.None
                            );
                        if (null == textEncodingOut)
                        {
                            outs = new StreamWriter(fileStreamOut);
                        }
                        else
                        {
                            outs = new StreamWriter(fileStreamOut, textEncodingOut);
                        }
                    }
                    CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
                    CultureInfo cultureInfoIn = InvariantCulture;
                    CultureInfo cultureInfoOut = InvariantCulture;
                    CsvReader csvIn = new CsvReader(ins, csvEncodingIn);
                    CsvWriter csvOut = new CsvWriter(outs, csvEncodingOut);
                    StringComparer csvFieldValueComparer = StringComparer.Ordinal;
                    IComparer csvFieldValueSortComparer = csvFieldValueComparer;
                    IEqualityComparer csvFieldValueEqualityComparer = csvFieldValueComparer;
                    FieldSchemaSpec<object> specParser = new FieldSchemaSpec<object>();
                    StringBuilder specParserBuffer = new StringBuilder();
                    int startPosition = 0;
                    IList<string> fieldNameList = new List<string>();
                    IList<IRecordFieldType<object>> fieldTypeList = new List<IRecordFieldType<object>>();

                    // read header row and get field names and type info
                    if (csvIn.ReadRecord())
                    {
                        while (csvIn.ReadValue())
                        {
                            string fieldSpec = csvIn.ValueText;
                            KeyValuePair<string,FieldSchemaSpecFieldRecord<object>> fieldInfo
                                = specParser.ParseField(fieldSpec, startPosition, specParserBuffer);
                            string fieldName = fieldInfo.Key;
                            FieldSchemaSpecFieldRecord<object> fieldType = fieldInfo.Value;
                            fieldNameList.Add(fieldName);
                            fieldTypeList.Add(fieldType);
                        }

                        // write header row to output CSV
                        csvOut.WriteStartRecord();
                        for (int i = 0; i < fieldNameList.Count && i < fieldTypeList.Count; i++)
                        {
                            string fieldName = fieldNameList[i];
                            FieldSchemaSpecFieldRecord<object> fieldType = (FieldSchemaSpecFieldRecord<object>)fieldTypeList[i];
                            string fieldTypeName = fieldType.FieldTypeName;
                            string fieldSpecString = String.Format("{0}:{1}",
                                fieldName
                                ,fieldTypeName
                            );
                            csvOut.WriteValue(fieldSpecString);
                        }
                        csvOut.WriteEndRecord();
                    }

                    IRecordEnumerator<string> csvInRecordEnumerator = new CsvRecordEnumerator(
                        csvIn
                        ,csvFieldValueComparer
                        ,csvFieldValueComparer
                        ,fieldNameList
                    );
                    IRecordCollectionBuilder<string> csvOutRecordCollectionBuilder = new CsvRecordCollectionBuilder(
                        csvOut
                        ,csvFieldValueComparer
                        ,csvFieldValueComparer
                        ,fieldNameList
                    );

                    if (!shouldParseFieldValues)
                    {
                        RecordIO.ReadInto(
                            csvOutRecordCollectionBuilder
                            ,csvInRecordEnumerator
                        );
                    }
                    else
                    {
                        IRecordAccessor<IRecordFieldType<object>> fieldSchema = new ListRecordAccessor<IRecordFieldType<object>,IRecordFieldType<IRecordFieldType<object>>>(
                            fieldTypeList
                            ,fieldNameList
                            ,null
                            );
                        IRecordAccessorAdapter<object,IRecordAccessor<string>> inRecordAdapter = new ParsingRecordAccessor<IRecordFieldType<object>>(
                            fieldSchema
                            ,cultureInfoIn
                        );
                        IRecordEnumerator<object> inRecordEnumerator = new RecordEnumeratorAdapter<object,IRecordAccessor<string>>(
                            inRecordAdapter
                            ,csvInRecordEnumerator
                        );
                        IRecordAccessorAdapter<string,IRecordAccessor<object>> outRecordAdapter = new PrintingRecordAccessor<object,IRecordFieldType<object>>(
                            fieldSchema
                            ,cultureInfoOut
                        );
                        IRecordEnumerator<string> outRecordEnumerator = new RecordEnumeratorAdapter<string,IRecordAccessor<object>>(
                            outRecordAdapter
                            ,inRecordEnumerator
                        );
                        RecordIO.ReadInto(
                            csvOutRecordCollectionBuilder
                            ,outRecordEnumerator
                        );
                    }

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
                }
            }

            return exitCode;
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

    } // /class

} // /namespace
