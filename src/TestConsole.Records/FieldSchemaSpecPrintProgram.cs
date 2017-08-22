/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

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
         "field_schema_spec-print tool version 20170308\n"
        +"Copyright (c) 2017 Upstream Research, Inc.\n"
        +"\n"
        +"field_schema_spec-print FieldSchemaSpec\n"
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
            TextWriter outs = stdout;
            TextWriter errs = stderr;
            FieldSchemaSpec<object> specParser = new FieldSchemaSpec<object>();
            IEnumerable<KeyValuePair<string,FieldSchemaSpecFieldRecord<object>>> fieldEnumeration;
            string fieldSpecString = null;
            IEnumerator<string> argsEnum = args.GetEnumerator();

            if (argsEnum.MoveNext())
            {
                fieldSpecString = argsEnum.Current;
            }
            if (null == fieldSpecString
                || 0 == String.Compare(fieldSpecString, "--help", StringComparison.InvariantCulture)
                )
            {
                errs.Write(HelpText);
            }
            else
            {
                fieldEnumeration = specParser.ParseEnumerable(fieldSpecString);
                foreach (KeyValuePair<string,FieldSchemaSpecFieldRecord<object>> fieldPair in fieldEnumeration)
                {
                    string fieldName = fieldPair.Key;
                    FieldSchemaSpecFieldRecord<object> fieldType = fieldPair.Value;
                    Type dataType = fieldType.DataType;
                    string dataTypeName = dataType.ToString();

                    outs.WriteLine(String.Format("{0}  {1}"
                        ,fieldName
                        ,dataTypeName)
                        );
                }
            }
            
            return exitCode;
        }

    } // /class
} // /namespace
