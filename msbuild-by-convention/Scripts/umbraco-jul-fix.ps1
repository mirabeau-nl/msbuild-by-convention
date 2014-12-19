Param(
	[Parameter(Mandatory=$true)] $path
)
 
$proxyHtm = @"
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Repo proxy</title>
</head>
<body>
 
    <script type="text/javascript">
        
        //This is a genius way of parsing a uri
        //https://gist.github.com/jlong/2428561
 
        try {
            var parser = document.createElement('a');
            parser.href = window.location.search.substring(1);
 
            // => "http:"
            if (!parser.protocol || (parser.protocol.toLowerCase() != "http:" && parser.protocol.toLowerCase() != "https:")) {
                throw "invalid protocol";
            };
 
            // => "example.com"
            if (!parser.hostname || parser.hostname == "") {
                throw "invalid hostname";
            }
            
            //parser.port;     // => "3000"
 
            // => "/pathname/"
            if (!parser.pathname || ((parser.pathname.length - parser.pathname.indexOf("/developer/packages/installer.aspx")) != "/developer/packages/installer.aspx".length))
            {
                throw "invalid pathname";
            }
            
            // => "?search=test"
            if (!parser.search || parser.search.indexOf("?repoGuid") != 0) {
                throw "invalid search";
            }
            
            // => "#hash"
            if (parser.hash && parser.hash != "") {
                throw "invalid hash";
            }   
 
            //parser.host;     // => "example.com:3000"
 
            if (!top.right) {
                throw "invalid document";
            }
 
            top.right.document.location = window.location.search.substring(1);
 
        } catch (e) {
            alert(e);
        }
 
        
    </script>
 
</body>
</html>
"@
 
$scriptLocation = Get-Location
Set-Location -Path $path
 
# update proxy.htm
if (Test-Path "umbraco\Developer\Packages\proxy.htm") {
    Out-File -FilePath "umbraco\Developer\Packages\proxy.htm" -Encoding "UTF8" -InputObject $proxyHtm
}
 
if (Test-Path "umbraco\Dashboard\Swfs\AIRInstallBadge.swf") {
    Remove-Item "umbraco\Dashboard\Swfs\AIRInstallBadge.swf"
}
 
if (Test-Path "Config\Splashes\booting.aspx") {
    Remove-Item "Config\Splashes\booting.aspx"
}
if (Test-Path "install") {
    Remove-Item "install" -recurse -force
}