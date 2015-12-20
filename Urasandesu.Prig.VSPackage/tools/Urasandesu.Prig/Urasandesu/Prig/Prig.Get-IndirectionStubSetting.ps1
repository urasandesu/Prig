# 
# File: Prig.Get-IndirectionStubSetting.ps1
# 
# Author: Akira Sugiura (urasandesu@gmail.com)
# 
# 
# Copyright (c) 2012 Akira Sugiura
#  
#  This software is MIT License.
#  
#  Permission is hereby granted, free of charge, to any person obtaining a copy
#  of this software and associated documentation files (the "Software"), to deal
#  in the Software without restriction, including without limitation the rights
#  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#  copies of the Software, and to permit persons to whom the Software is
#  furnished to do so, subject to the following conditions:
#  
#  The above copyright notice and this permission notice shall be included in
#  all copies or substantial portions of the Software.
#  
#  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#  THE SOFTWARE.
#

function Get-IndirectionStubSetting {
<#
    .SYNOPSIS
        Gets the Indirection Stub Setting on the Package Manager Console, PowerShell or the Prig Setup Session.

    .DESCRIPTION
        This command creates XML tags that can be used as the Indirection Stub Setting for the target method on the Package Manager Console, PowerShell or the Prig Setup Session, and gets them.

        The Indirection Stub Setting mean the tag that can be added to the Stub Settings File(`*.prig`) that is set up by the command `Add-PrigAssembly`. In particular, the tag is `add`. You can insert it to between the tag `<stubs>...</stubs>` of the file.

        Note that you have to typically generate unique name, and set to the tag(its attribute `name` and `alias`) in the type the target method is declared. However, you can get easily such name by using this command.

        These generated types are called Prig Type or Prig Type Introducer. Prig Type is a stub type to set the dummy method body as replacement target method, and Prig Type Introducer is the type that defines the identifier list to specify the target method. Their naming convention is similar to Microsoft Fakes's, but there is also a little different:  
        * The namespace that the Prig Types are located is the original namespace + `.Prig`.  
          For example, the Prig Types for the types under `System` are located at the namespace `System.Prig`.

        * The prefix of the Prig Type is `P`(no conditions) or `PProxy`(specified instance of a class).  
          For example, the Prig Type for `System.DateTime` is `System.Prig.PDateTime`. However, the Prig Type `PProxyDateTime` isn't generated, because it is a structure.
          The Prig Type for `System.Net.HttpWebRequest` is `System.Net.Prig.PHttpWebRequest`. Also, `System.Net.Prig.PProxyHttpWebRequest` is generated, because it is a class.

        * The prefix of the Prig Type Introducer is `OfP`(no conditions) or `OfPProxy`(specified instance of a class).  
          The difference between a class and a structure is same as Prig Type.

        See also [Code generation, compilation, and naming conventions in Microsoft Fakes](http://msdn.microsoft.com/en-us/library/hh708916.aspx).

        The results are output as plain text, so I recommend that you use in combination with `clip` command.

    .PARAMETER  InputObject
        A array of `System.Reflection.MethodBase` object which is target to get the Indirection Stub Setting.

    .EXAMPLE
        PS C:\>pfind datetime _now | pget
        <add name="NowGet" alias="NowGet">
        <RuntimeMethodInfo xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns:x="http://www.w3.org/2001/XMLSchema" z:Id="1" z:FactoryType="MemberInfoSerializationHolder" z:Type="System.Reflection.MemberInfoSerializationHolder" z:Assembly="0" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/System.Reflection">
          <Name z:Id="2" z:Type="System.String" z:Assembly="0" xmlns="">get_Now</Name>
          <AssemblyName z:Id="3" z:Type="System.String" z:Assembly="0" xmlns="">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id="4" z:Type="System.String" z:Assembly="0" xmlns="">System.DateTime</ClassName>
          <Signature z:Id="5" z:Type="System.String" z:Assembly="0" xmlns="">System.DateTime get_Now()</Signature>
          <Signature2 z:Id="6" z:Type="System.String" z:Assembly="0" xmlns="">System.DateTime get_Now()</Signature2>
          <MemberType z:Id="7" z:Type="System.Int32" z:Assembly="0" xmlns="">8</MemberType>
          <GenericArguments i:nil="true" xmlns="" />
        </RuntimeMethodInfo>
        </add>
        
        PS C:\>pfind datetime _now | pget | clip
        
        DESCRIPTION
        -----------
        This is the example that is used in combination with `Find-IndirectionTarget`. Search the members that are matched to the regular expression `_now` from the type `System.DateTime`, and get the Indirection Stub Setting. When confirmed that, it has no problem. So, copy it to clipboard.
        Usually, paste it to the Stub Settings File thereafter.

    .INPUTS
        System.Reflection.MethodBase, System.Reflection.MethodBase[]

    .OUTPUTS
        System.String

    .NOTES
        You have to import the module `Urasandesu.Prig` explicitly if you use this command on PowerShell directly. The module `Urasandesu.Prig` is placed the directory `tools` of the package directory by NuGet when you installed Prig. So, execute `Import-Module` from there. By the way, this step requires a little labor. Using the Prig Setup Session would be more easier. See also the help for `Start-PrigSetup` for more details.
        
        You can also refer to the Get-IndirectionStubSetting command by its built-in alias, "PGet".

    .LINK
        Add-PrigAssembly

    .LINK
        Find-IndirectionTarget

    .LINK
        Start-PrigSetup

#>

    [CmdletBinding()]
    param (
        [Parameter(ValueFromPipeline = $true)]
        [System.Reflection.MethodBase[]]
        $InputObject
    )

    begin {
        [Void][System.Reflection.Assembly]::LoadWithPartialName('System.Runtime.Serialization')
    } process {
        if ($null -ne $InputObject) {
            foreach ($methodBase in $InputObject) {
                $ndcs = New-Object System.Runtime.Serialization.NetDataContractSerializer
                $sb = New-Object System.Text.StringBuilder
                $sw = New-Object System.IO.StringWriter $sb
                try {
                    $xw = New-Object System.Xml.XmlTextWriter $sw
                    $xw.Formatting = [System.Xml.Formatting]::Indented
                    $ndcs.WriteObject($xw, $methodBase);
                } finally {
                    if ($null -ne $xw) { $xw.Close() }
                }

                $content = $sb.ToString()
                $name = ConvertToIndirectionStubName $methodBase
                @"
<add name="$name" alias="$name">
$content
</add>

"@
            }
        }
    } end {

    }
}

New-Alias PGet Get-IndirectionStubSetting
