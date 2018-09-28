/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;
using System.IO;

namespace Upstream.System.Records
{
    /// <summary>
    /// Test program that parses a records library field spec
    /// </summary>
    public class FieldSchemaSpecPrintProgram
    {
        const string HelpText = 
         "field_schema_spec-print tool version 20170308:20180811\n"
        +"Copyright (c) 2017 Upstream Research, Inc.\n"
        +"\n"
        +"field_schema_spec-print FieldSchemaSpec\n"
        +"    -s <S>   name/datatype delimiter for output (default=',')\n"
        +"\n"
        ;
        
        internal static int
        Main(string[] args)
        {
            return Main(
                args
                ,Console.In
                ,Console.Out
                ,Console.Error
                );
        }

        internal static int
        Main(
            IEnumerable<string> args
            ,TextReader stdin
            ,TextWriter stdout
            ,TextWriter stderr
            )
        {
            int exitCode = 0;
            string dataTypeSeparator = ",";
            TextWriter outs = stdout;
            TextWriter errs = stderr;
            string arg;
            bool showHelp = false;
            FieldSchemaSpecEncoding<object> specParser = new FieldSchemaSpecEncoding<object>("unknown");
            IEnumerable<IFieldNameValuePair<FieldSchemaSpecFieldType<object>>> fieldEnumeration;
            string fieldSpecString = null;
            IEnumerator<string> argsEnum = args.GetEnumerator();

            while (argsEnum.MoveNext())
            {
                arg = argsEnum.Current;
                if (ArgOptionEquals("--help", arg))
                {
                    showHelp = true;
                }
                else if (ArgOptionEquals("--separator", arg)
                    || ArgOptionEquals("-s", arg)
                    )
                {
                    if (argsEnum.MoveNext())
                    {
                        arg = argsEnum.Current;
                        dataTypeSeparator = ParseCsvSeparatorArg(arg);
                    }
                }
                else if (ArgOptionEquals("--empty", arg))
                {
                    fieldSpecString = String.Empty;
                }
                else if (ArgOptionEquals("--null", arg))
                {
                    fieldSpecString = null;
                }
                else if (
                    !ArgIsOption(arg)
                    && (null == fieldSpecString)
                    )
                {
                    fieldSpecString = argsEnum.Current;
                }
            }
            if (showHelp)
            {
                errs.Write(HelpText);
            }
            else
            {
                fieldEnumeration = specParser.DecodeEnumerable(fieldSpecString);
                foreach (IFieldNameValuePair<IRecordFieldType<object>> fieldPair in fieldEnumeration)
                {
                    string fieldName = fieldPair.Name;
                    IRecordFieldType<object> fieldType = fieldPair.Value;
                    Type dataType = fieldType.SystemType;
                    string dataTypeName = "<unknown>";
                    if (null != dataType)
                    {
                        dataTypeName = dataType.ToString();
                    }

                    outs.WriteLine(String.Format("{1}{0}{2}"
                        ,dataTypeSeparator
                        ,fieldName
                        ,dataTypeName)
                        );
                }
            }
            
            return exitCode;
        }

        private static bool 
        ArgIsOption(string arg)
        {
            if (null == arg)
            {
                return false;
            }

            return arg.StartsWith("-", StringComparison.Ordinal);
        }

        private static bool 
        ArgOptionEquals(string arg, string arg2)
        {
            return (0 == String.Compare(arg, arg2, StringComparison.Ordinal));
        }

        private static string 
        ParseCsvSeparatorArg(string arg)
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

    } // /class
} // /namespace
