MultiMiner
==========
### Your coins. Your pools. Your way.
__MultiMiner__ is a graphical application for crypto-coin mining. MultiMiner simplifies switching individal devices (GPUs, ASICs, FPGAs) between crypto-currencies such as Bitcoin and Litecoin.

MultiMiner uses the underlying mining engine ([cgminer][1] by default) to detect available mining devices and then presents a user interface for selecting the coins you'd like to mine.

![Main Screen](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Main%20Screen.png "Main Screen")

MultiMiner will automatically download and install the latest version of either cgminer, [bfgminer][2], or both, making it simple for the new user to get started.

![No Miners Prompt](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/No%20Miners%20Prompt.png "No Miners Prompt")

These command line miners are standard tools for crypto-coin mining and are downloaded from official sources.

![Downloading and Installing Cgminer](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Downloading%20and%20Installing%20Cgminer.png "Downloading and Installing Cgminer")

You can then use the Configure Coins dialog to setup each coin that you would like to mine along with their pools, including support for load balancing.

![Configure Coins](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Configure%20Coins.png "Configure Coins")

MultiMiner supports automatically mining the most profitable coins based on a set of configurable strategies. Profitability information is updated regularly from [CoinChoose.com][9].

![Configure Strategies](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Configure%20Strategies.png "Configure Strategies")

MultiMiner also supports features such as relaunching crashed miners, starting with Windows, minimizing to the notification area, and mining on startup.

![Settings](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Settings.png "Settings")

Finally, MultiMiner supports [MobileMiner][14], an open API with mobile apps for remotely monitoring and controlling your rigs.

![MobileMiner](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/MobileMiner.png "MobileMiner")

By entering your MobileMiner email address and application key in the Configure Settings dialog, you will be able to remotely monitor and control your rigs _without having to open any firewalls or forward any ports_.

Downloads
----------------
You can download installers and zip files for Windows, OS X, and Mono on the [GitHub Releases page][12].

Windows Installation
--------------------
1. Download and run the installer (.exe) file at the above link and follow instructions

The installer runs without needing admin rights and does not install to Program Files so as not to be intrusive. However, if you prefer you can use the zip file:

1. Download and extract the .zip file at the above link
2. Launch MultiMiner.Win.exe to get started

OS X Installation
-----------------
1. Install X11. Under OS X you should install Xquartz available [here][7]
2. Install the latest version of [Mono][8]
3. Download and extract the __.app__.zip file at the above Binary Downloads link
4. Launch MultiMiner.app to get started

![No Miners Prompt - OS X](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/No%20Miners%20Prompt%20-%20OS%20X.png "No Miners Prompt - OS X")

MultiMiner will automatically download redistributable binaries of cgminer and bfgminer from the [xgminer-osx][13] project.

![Main Screen - OS X](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Main%20Screen%20-%20OS%20X.png "Main Screen - OS X")

Mono (OS X, Linux) Installation
-------------------------------
1. Download and extract the zip file at the above Binary Downloads link
2. Install cgminer or bfgminer. For OS X, you can find packages and for doing so [here][5] and instructions for using them [here][6].
3. Install X11. Under OS X you should install Xquartz available [here][7].
4. Install the latest version of [Mono][8].
5. Run MultiMiner.Win.exe using mono:

        mono MultiMiner.Win.exe
        
Source Code
-----------
The source code is structured in such a way that it should be fairly easy to use and re-use for other projects:

* __MultiMiner.Xgminer__ is an assembly for controling either the cgminer or bfgminer executable - e.g. launching and enumerating devices
* __MultiMiner.Xgminer.Api__ assists in communicating with the underlying miner via the RPC API
* __MultiMiner.Coinchoose.Api__ assists in consuming the cypto-currency information available at [CoinChoose.com][9]
* __MultiMiner.Engine__ is an assembly that can be used to interact with all functionality found in MultiMiner, but without a UI - useful for creating front-ends for other OS's
* __MultiMiner.Win__ is the Windows Forms application

License
-------
Copyright (C) 2013 Nathanial Woolls

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


[1]: https://github.com/ckolivas/cgminer
[2]: https://github.com/luke-jr/bfgminer
[3]: https://www.dropbox.com/s/ne5eywfx8v7hneb/MultiMiner-1.0.7.zip
[4]: http://ck.kolivas.org/apps/cgminer/
[5]: https://github.com/nwoolls/homebrew-cryptocoin
[6]: http://blog.nwoolls.com/2013/04/24/bitcoin-mining-on-mac-os-x-cgminer-bfgminer/
[7]: http://xquartz.macosforge.org/
[8]: http://www.mono-project.com/Main_Page
[9]: http://coinchoose.com/
[10]: http://luke.dashjr.org/programs/bitcoin/files/bfgminer/
[11]: https://www.dropbox.com/s/o08inghtw7ut1an/MultiMiner-1.0.7.exe
[12]: https://github.com/nwoolls/MultiMiner/releases
[13]: https://github.com/nwoolls/xgminer-osx
[14]: http://www.mobileminerapp.com
