# Log4j-Windows-Scanner
CVE-2021-44228 vulnerability in Apache Log4j library | Log4j vulnerability scanner on Windows machines.


## How does it work

It detects the vulnerable versions of log4j by following the folders in the C Directory. 

When it exposes vulnerable files, it reveals ".TXT" files as output. Unable to scan file "log.txt". 

In other words, it will list the directories whose contents cannot be read because it cannot access a directory due to its privileges. 

Please follow these folders. Make sure you run it with administrator privilege. 

At the next stage, it will search inside the "JndiLookup.class" file in vulnerable versions.

![alt text](https://i.imgur.com/CmRuyIX.png)


## Usage

```none
on any command screen > Log4j.exe
```


## Suggestion

It is useful to remember that. With this scanner, it only makes short-term scans and tries to guess the vulnerable files by acting on the versions announced today. 
Please do your necessary simultaneous tests.


## CVE-2021-44228

- https://cve.mitre.org/cgi-bin/cvename.cgi?name=CVE-2021-44228

- https://nvd.nist.gov/vuln/detail/CVE-2021-44228

This is not PoC

## Contact

Twitter: [@oulusoyum](https://twitter.com/oulusoyum)
