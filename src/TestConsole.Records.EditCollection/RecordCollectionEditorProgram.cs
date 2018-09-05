/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License.  See LICENSE file in top-level directory.  */

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

using Upstream.System.Csv;
using Upstream.System.Records.Csv;

namespace Upstream.System.Records
{
    class RecordCollectionEditorProgram
    {
        const string HelpText = 
         "rcd-edit tool version 20180905\n"
        +"\n"
        +"rcd-edit [OPTIONS] [InputFile]\n"
        +"\n"
        +"OPTIONS\n"
        +"    -E {E}  Input file text encoding (e.g. 'utf-8', 'windows-1252')\n"
        +"    -e {E}  Output file text encoding (e.g. 'utf-8', 'windows-1252')\n"
        +"    -L {L}  Input file culture (language/locale) (e.g. 'en-US', 'fr-FR')\n"
        +"    -l {L}  Output file culture (language/locale) (e.g. 'en-US', 'fr-FR')\n"
        +"    -o {F}  Output file name\n"
        +"    -S {S}  Input file field delimiter\n"
        +"    -s {S}  Output file field delimiter\n"
        +"    -W,-w   Read-write mode (output file is same as input file)\n"
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
            bool shouldParseFieldValues = false;
            TextReader ins = Console.In;
            TextWriter outs = Console.Out;
            TextWriter errs = Console.Error;
            string errorMessage = null;



            return exitCode;
        }
        
    } // /class

}  // /namespace
