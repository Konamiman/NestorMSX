using System;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Host
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            var header = @"
<html>
<head><style type=""text/css"">
body { font-family: sans-serif; font-size: x-small; }
a { text-decoration: none }
</style></head>
<body>";

            webAbout.DocumentText = header + @"
<h4>NestorMSX 2.0</h4>
<p>(c) 2015, 2016 Konamiman - <a target=""_blank"" href=""http://www.konamiman.com"">http://www.konamiman.com</a></p>
<br/>
<p>Source code and configuration guide: <a target=""_blank"" href=""https://bitbucket.org/konamiman/nestormsx"">https://bitbucket.org/konamiman/nestormsx</a></p>
<p>Uses Z80.NET: <a target=""_blank"" href=""https://bitbucket.org/konamiman/z80dotnet"">https://bitbucket.org/konamiman/z80dotnet</a></p>
<br/>
<p><strong><a target=""_blank"" href=""http://konamiman.com/msx/msx-e.html#donate"">Donations are welcome!</a></strong></p>
</body></html>
";

            webCommandLine.DocumentText = header + @"
<p>NestorMSX accepts the following command line arguments:</p>
<p><b>keytest</b>: Run in key test mode. Useful if you want to create a key mappings file or modify a existing one.</p>
<p><b>machine=""<em>name</em>""</b>: Start the emulator with the specified machine.</p>
<p><b>sc</b>: Show the debug console. Useful to debug plugins.</p>
<p><b>wd</b>: Wait until a debugger is attached before starting emulation. Useful to debug plugins.</p>
";
        }
    }
}
