/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

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
    /// Command to project a selection of columns from a CSV input stream
    /// </summary>
    // (20180925 (db) ORIGIN: CsvRecordTranslateProgram.cs)
    class CsvRecordSelectionProgram
    {
        const string HelpText = 
         "RCD-CSV-SELECT tool version 201800925\n"
        +"\n"
        +"rcd-csv-select [OPTIONS] FieldList [InputFile]\n"
        +"\n"
        +"OPTIONS\n"
        +"    -E {E}  Input file text encoding (e.g. 'utf-8', 'windows-1252')\n"
        +"    -e {E}  Output file text encoding (e.g. 'utf-8', 'windows-1252')\n"
        +"    -L {L}  Input file culture (language/locale) (e.g. 'en-US', 'fr-FR')\n"
        +"    -l {L}  Output file culture (language/locale) (e.g. 'en-US', 'fr-FR')\n"
        +"    -o {F}  Output file name\n"
        +"    -S {S}  Input file field delimiter\n"
        +"    -s {S}  Output file field delimiter\n"
        +"\n"
        +"<FieldList> is a comma-separated list of field names to select from the \n"
        +"input stream.  Fields may optionally be prefixed by an 'alias' name followed\n"
        +"by an equals symbol as in the form '<Alias>=<Name>'.\n"
        +"\n"
        +"This program will parse input values according to a 'schema' when the\n"
        +"header row contains a 'field specification'.\n"
        ;

        internal static 
        int Main(string[] args)
        {
            int exitCode = 1;
            CsvRecordSelectionProgram exe = new CsvRecordSelectionProgram();

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
            string cultureNameIn = null;
            string cultureNameOut = null;
            CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
            CultureInfo cultureInfoIn = InvariantCulture;
            CultureInfo cultureInfoOut = InvariantCulture;
            TextReader ins = Console.In;
            TextWriter outs = Console.Out;
            TextWriter errs = Console.Error;
            string errorMessage = null;
            //bool shouldParseFieldValues = false;
            string fieldSelectionString = null;
            IList<string> selectedFieldNameList = null;
            IList<string> selectedFieldAliasList = null;
            const char fieldListSeparatorChar = ',';
            const string fieldAliasSeparator = "=";

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
                    // (20180925 (db) this is not optional; we will always parse the input fields)
                    //shouldParseFieldValues = true;
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
                    if (null == fieldSelectionString)
                    {
                        fieldSelectionString = arg;
                    }
                    else if (null == fileNameIn)
                    {
                        fileNameIn = arg;
                    }
                }
            }

            if (null == fieldSelectionString)
            {
                errorMessage = "Missing field list";
            }
            else
            {
                IList<string> fieldSelectorList = fieldSelectionString.Split(fieldListSeparatorChar);
                int fieldCountEstimate = fieldSelectorList.Count;
                selectedFieldNameList = new List<string>(fieldCountEstimate);
                selectedFieldAliasList = new List<string>(fieldCountEstimate);
                foreach (string fieldSelector in fieldSelectorList)
                {
                    string fieldName = fieldSelector;
                    string fieldAlias = fieldName;
                    int aliasSeparatorPosition = fieldSelector.IndexOf(fieldAliasSeparator);
                    if (0 < aliasSeparatorPosition
                        && aliasSeparatorPosition < fieldSelector.Length
                        )
                    {
                        fieldAlias = fieldSelector.Substring(0, aliasSeparatorPosition);
                        fieldName = fieldSelector.Substring(aliasSeparatorPosition + fieldAliasSeparator.Length);
                    }
                    selectedFieldNameList.Add(fieldName);
                    selectedFieldAliasList.Add(fieldAlias);
                }
            }

            if (!String.IsNullOrEmpty(cultureNameIn))
            {
                cultureInfoIn = FindCultureInfo(cultureNameIn);
                if (null == cultureInfoIn)
                {
                    errorMessage = String.Format("Unknown culture name: '{0}'", cultureNameIn);
                }
            }
            if (!String.IsNullOrEmpty(cultureNameOut))
            {
                cultureInfoOut = FindCultureInfo(cultureNameOut);
                if (null == cultureInfoOut)
                {
                    errorMessage = String.Format("Unknown culture name: '{0}'", cultureNameOut);
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
                    CsvReader csvIn = new CsvReader(ins, csvEncodingIn);
                    CsvWriter csvOut = new CsvWriter(outs, csvEncodingOut);
                    StringComparer csvFieldValueComparer = StringComparer.Ordinal;
                    IComparer<string> csvFieldValueSortComparer = csvFieldValueComparer;
                    IEqualityComparer<string> csvFieldValueEqualityComparer = csvFieldValueComparer;
                    FieldSchemaSpecEncoding<object> fieldSpecEncoding = new FieldSchemaSpecEncoding<object>();
                    StringBuilder specParserBuffer = new StringBuilder();
                    //int startPosition = 0;
                    IRecordSchemaViewer<FieldSchemaSpecFieldType<object>>
                        recordSchemaIn = new BasicRecordSchema<FieldSchemaSpecFieldType<object>>();
                    //IList<string> fieldNameListIn = new List<string>();
                    //IList<IRecordFieldType<object>> fieldTypeListIn = new List<IRecordFieldType<object>>();

                    // read header row and get field names and type info
                    if (csvIn.ReadRecord())
                    {
                        recordSchemaIn = fieldSpecEncoding.DecodeRecordSchema(csvIn);
                    }


                    IFieldNameValuePair<FieldSchemaSpecFieldType<object>> defaultFieldInfo = fieldSpecEncoding.DecodeField("default:varchar");
                    FieldSchemaSpecFieldType<object> defaultFieldType = defaultFieldInfo.Value;
                    FieldSelectionRecordAccessorAdapter<object,FieldSchemaSpecFieldType<object>> 
                        fieldSelectionAdapter = new FieldSelectionRecordAccessorAdapter<object,FieldSchemaSpecFieldType<object>>(
                            recordSchemaIn
                            ,defaultFieldType
                            );
                    for (int fieldPosition = 0;
                        fieldPosition < selectedFieldNameList.Count
                        && fieldPosition < selectedFieldAliasList.Count
                        ;fieldPosition++
                        )
                    {
                        string fieldName = selectedFieldNameList[fieldPosition];
                        string fieldAlias = selectedFieldAliasList[fieldPosition];
                        fieldSelectionAdapter.AddField(fieldAlias, fieldName);
                    }

                    IRecordSchemaViewer<FieldSchemaSpecFieldType<object>> recordSchemaOut = fieldSelectionAdapter.RecordSchema;
                    if (0 < recordSchemaOut.GetFieldCount())
                    {
                        // write header row to output CSV
                        csvOut.WriteStartRecord();
                        foreach (IFieldNameValuePair<FieldSchemaSpecFieldType<object>> fieldInfo in recordSchemaOut)
                        {
                            string fieldName = fieldInfo.Name;
                            FieldSchemaSpecFieldType<object> fieldType = fieldInfo.Value;
                            string fieldTypeName = fieldType.FieldTypeName;
                            string fieldSpecString = fieldName;
                            if (!String.IsNullOrEmpty(fieldTypeName))
                            {
                                fieldSpecString = fieldSpecEncoding.EncodeField(fieldName, fieldType);
                            }
                            csvOut.WriteValue(fieldSpecString);
                        }
                        csvOut.WriteEndRecord();
                    }

                    IRecordEnumerator<string> csvInRecordEnumerator = new CsvRecordEnumerator(
                        csvIn
                        ,recordSchemaIn.FieldNames
                        ,csvFieldValueSortComparer
                        ,csvFieldValueEqualityComparer
                    );
                    IRecordCollectionBuilder<string> csvOutRecordCollectionBuilder = new CsvRecordCollectionBuilder(
                        csvOut
                        ,selectedFieldAliasList
                        ,csvFieldValueSortComparer
                        ,csvFieldValueEqualityComparer
                    );

                    IRecordAccessorAdapter<object,IRecordAccessor<string>> inRecordAdapter = new ParsingRecordAccessor<IRecordFieldType<object>>(
                         recordSchemaIn
                        ,cultureInfoIn
                    );
                    IRecordEnumerator<object> inRecordEnumerator = new RecordEnumeratorAdapter<object,IRecordAccessor<string>>(
                        inRecordAdapter
                        ,csvInRecordEnumerator
                    );
                    IRecordEnumerator<object> selectionRecordEnumerator = new RecordEnumeratorAdapter<object,IRecordAccessor<object>>(
                        fieldSelectionAdapter
                        ,inRecordEnumerator
                        );
                    IRecordAccessorAdapter<string,IRecordAccessor<object>> outRecordAdapter = new PrintingRecordAccessor<object,IRecordFieldType<object>>(
                        recordSchemaOut
                        ,cultureInfoOut
                    );
                    IRecordEnumerator<string> outRecordEnumerator = new RecordEnumeratorAdapter<string,IRecordAccessor<object>>(
                        outRecordAdapter
                        ,selectionRecordEnumerator
                    );
                    RecordIO.ReadInto(
                        csvOutRecordCollectionBuilder
                        ,outRecordEnumerator
                    );

                }
                catch (Exception ex)
                {
                    errorMessage = ex.ToString();
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

    } // /class

} // /namespace
