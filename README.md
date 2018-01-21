## MultiMiner

### Your coins. Your pools. Your way.

**MultiMiner** is a graphical application for crypto-coin mining on Windows, OS X and Linux. MultiMiner simplifies switching individual devices (**GPUs**, **ASICs**, **FPGAs**, **CPUs**) between crypto-currencies such as Bitcoin, Litecoin, Ethereum, Monero, Zcash, and more, while also allowing you to manage any mining appliances on your network (**AntMiners**, **G-Black**, Raspberry **Pi controllers**, **Spondoolies** and more).

* [Installation](https://github.com/nwoolls/MultiMiner/wiki/Installation)
* [Getting Started](https://github.com/nwoolls/MultiMiner/wiki/Getting-Started)
* [Local Devices](https://github.com/nwoolls/MultiMiner/wiki/Local-Devices)
* [Network Devices](https://github.com/nwoolls/MultiMiner/wiki/Network-Devices)
* [Remote Management](https://github.com/nwoolls/MultiMiner/wiki/Remote-Management)
* [Contributing](https://github.com/nwoolls/MultiMiner/wiki/Contributing)
* [Tips & Tricks](https://github.com/nwoolls/MultiMiner/wiki/Tips-and-Tricks)

MultiMiner uses the underlying mining engine ([BFGMiner](https://github.com/luke-jr/bfgminer)) to detect available mining devices and then presents a user interface for selecting the coins you'd like to mine.

![Main Screen](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Main%20Screen.png "Main Screen")

MultiMiner supports mining coins that use the following algorithms out-of-the-box:

* SHA256
* Scrypt
* CryptoNight
* Equihash
* Ethash
* Pascal
* Groestl
* Keccak
* Lyra2RE
* NeoScrypt
* Quark
* Scrypt-Jane
* Scrypt-N
* X11
* X13
* X14
* X15

Additionally, MultiMiner allows the user to add any unsupported algorithm, coin, and miner, as long as there is a CGMiner, BFGMiner or SGMiner fork that supports the algorithm.

See the following topics for more information on adding unsupported algorithms:

* [GPU Algorithms](https://github.com/nwoolls/MultiMiner/wiki/GPU-Algorithms)
* [Backend Miners](https://github.com/nwoolls/MultiMiner/wiki/Backend-Miners)

MultiMiner ships with a [console application](https://github.com/nwoolls/MultiMiner/wiki/Console-Mode-(TUI)#console-mode-tui) (TUI) for low power devices such as ARM-based miners.

![Console Application](https://raw.githubusercontent.com/nwoolls/MultiMiner/master/Screenshots/TUI%20Main%20Screen.png "Console Application")

MultiMiner offers several views, allowing you to display as much or as little information as you like.

![Brief View](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Brief%20View.png "Brief View")

For new users, MultiMiner includes a [Getting Started](https://github.com/nwoolls/MultiMiner/wiki/Getting-Started#getting-started) wizard that walks you through selecting an engine, a coin, and a pool.

![Getting Started](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Getting%20Started.png "Getting Started")

MultiMiner will automatically download and install the latest version of [BFGMiner][2], making it simple for the new user to get started.

![Downloading and Installing Cgminer](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Downloading%20and%20Installing%20Bfgminer.png "Downloading and Installing Cgminer")

You can then use the [Configure Pools](https://github.com/nwoolls/MultiMiner/wiki/Pools#pools) dialog to setup each coin that you would like to mine along with their pools, including support for load balancing.

![Configure Coins](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Configure%20Coins.png "Configure Coins")

MultiMiner supports automatically mining the most profitable coins based on a set of configurable strategies. Profitability information is updated regularly from CoinWarz and WhatToMine.

![Configure Strategies](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Configure%20Strategies.png "Configure Strategies")

MultiMiner supports features such as relaunching crashed miners, starting with Windows, minimizing to the notification area, and mining on startup.

![Settings](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Settings.png "Settings")

You can also use the interface provided by MultiMiner to adjust advanced settings such as API white-listing, disabling GPU mining, and automatically adjusting mining intensity based on the computer's idle time.

![Advanced Miner Settings](https://github.com/nwoolls/MultiMiner/raw/master/Screenshots/Advanced%20Miner%20Settings.png "Advanced Miner Settings")

### Downloads

You can download installers and zip files for Windows, OS X, Linux and Mono on the [GitHub Releases page][12].

### Drivers

Depending on your OS and the mining devices you plan on using you will need one or more of the following drivers / kernel extensions installed:

* [Drivers](https://github.com/nwoolls/MultiMiner/wiki/Drivers)

### Installation

* [Windows](https://github.com/nwoolls/MultiMiner/wiki/Installation#windows)
* [Linux](https://github.com/nwoolls/MultiMiner/wiki/Installation#linux-installation-debian-based)
* [OS X](https://github.com/nwoolls/MultiMiner/wiki/Installation#mac-os-x)
* [ARM Installation (BeagleBone, Raspberry Pi)](https://github.com/nwoolls/MultiMiner/wiki/Installation#arm-installation-beaglebone-raspberry-pi)
* [Generic Mono](https://github.com/nwoolls/MultiMiner/wiki/Installation#generic-mono-installation)

### FAQ

* How do I restart the setup wizard? How do I delete all my MultiMiner settings and start over?
    * Reset settings by deleting the contents of %appdata%\MultiMiner (type that into the address bar in Windows Explorer). Be sure to quit MultiMiner before you delete the contents. Then, restart MultiMiner.
* Should I modify my GPU settings (overclock, undervolt, etc) with a utility like MSI Afterburner, or via config flags in MultiMiner?
    * Utilities like MSI Afterburner are recommended. Relying on BFGMiner (via the config flags in MultiMiner) to do the clocking and fan speed does work, but if the BFGMiner crashes it leaves things over/under-clocked/volted. GPU utilities seems to be more reliable.
* I found a question and answer in the Support thread below (or through gosh darned trial and error) that I wish had been here in the FAQ - how do I save the next visitor all the trouble I went through?
    * Simple! Make a pull request to add to the FAQ, or email your Q and A to cooper dot marcus at gmail dot com (Qs without As will be ignored) and he'll add it to the FAQ for you.

### Support

Please report bugs and request enhancements at [issues.multiminerapp.com](http://issues.multiminerapp.com). For peer support and discussion, official forums for MultiMiner can be found [here][15].
        
### Source Code

The source code is structured in such a way that it should be fairly easy to use and re-use for other projects:

* __MultiMiner.Xgminer__ is an assembly for controlling the BFGMiner executable - e.g. launching and enumerating devices
* __MultiMiner.Xgminer.Api__ assists in communicating with the underlying miner via the RPC API
* __MultiMiner.CoinWarz__ assists in consuming the cypto-currency information available at [CoinWarz.com](https://www.coinwarz.com/)
* __MultiMiner.WhatToMine__ assists in consuming the cypto-currency information available at [WhatToMine.com](http://whattomine.com/)
* __MultiMiner.Engine__ is an assembly that can be used to interact with all functionality found in MultiMiner, but without a UI - useful for creating front-ends for other OS's
* __MultiMiner.Win__ is the Windows Forms application

### Source Code Example

This simple example shows how to use [MultiMiner.Xgminer.dll][16] and [MultiMiner.Xgminer.Api.dll][17] to install BFGMiner, iterate through available mining devices, and launch the miner.

Afterwards the BFGMiner [RPC API][18] is used to output the miner hashrate for a minute before the mining process is stopped. You can try this code out yourself in the [MultiMiner.Example][19] project.

![MultiMiner.Api.Example](https://raw.github.com/nwoolls/MultiMiner/master/Screenshots/MultiMiner.Api.Example.png "MultiMiner.Api.Example")

```csharp
//examples of using MultiMiner.Xgminer.dll and MultiMiner.Xgminer.Api.dll

//download and install the latest version of BFGMiner
const string executablePath = @"D:\bfgminer\";
const string executableName = "bfgminer.exe";

Console.WriteLine("Downloading and installing {0} from {1} to the directory {2}",
    executableName, Xgminer.Installer.GetMinerDownloadRoot(), executablePath);

//download and install BFGMiner from the official website
Xgminer.Installer.InstallMiner(executablePath);
try
{
    //create an instance of Miner with the downloaded executable
    MinerConfiguration minerConfiguration = new MinerConfiguration()
    {
        ExecutablePath = Path.Combine(executablePath, executableName)
    };
    Miner miner = new Miner(minerConfiguration);

    //use it to iterate through devices
    List<Device> deviceList = miner.ListDevices();

    Console.WriteLine("Using {0} to list available mining devices", executableName);

    //output devices
    foreach (Device device in deviceList)
        Console.WriteLine("Device detected: {0}\t{1}\t{2}", device.Kind, device.Driver, device.Name);

    //start mining if there are devices
    if (deviceList.Count > 0)
    {
        Console.WriteLine("{0} device(s) detected, mining Bitcoin on Bitminter using all devices", deviceList.Count);

        //setup a pool
        MiningPool pool = new MiningPool()
        {
            Host = "mint.bitminter.com",
            Port = 3333,
            Username = "nwoolls_deepcore",
            Password = "deepcore"
        };
        minerConfiguration.Pools.Add(pool);

        //specify algorithm
        minerConfiguration.Algorithm = CoinAlgorithm.SHA256;

        //disable GPU mining
        minerConfiguration.DisableGpu = true;

        //specify device indexes to use
        for (int i = 0; i < deviceList.Count; i++)
            minerConfiguration.DeviceDescriptors.Add(deviceList[i]);

        //enable RPC API
        minerConfiguration.ApiListen = true;
        minerConfiguration.ApiPort = 4028;

        Console.WriteLine("Launching {0}", executableName);

        //start mining
        miner = new Miner(minerConfiguration);
        System.Diagnostics.Process minerProcess = miner.Launch();
        try
        {
            //get an API context
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(minerConfiguration.ApiPort);
            try
            {
                //mine for one minute, monitoring hashrate via the API
                for (int i = 0; i < 6; i++)
                {
                    Thread.Sleep(1000 * 10); //sleep 10s

                    //query the miner process via its RPC API for device information
                    List<Xgminer.Api.Responses.DeviceInformationResponse> deviceInformation = apiContext.GetDeviceInformation();

                    //output device information
                    foreach (Xgminer.Api.Responses.DeviceInformationResponse item in deviceInformation)
                        Console.WriteLine("Hasrate for device {0}: {1} current, {2} average", item.Index,
                                item.CurrentHashrate, item.AverageHashrate);
                }
            }
            finally
            {
                Console.WriteLine("Quitting mining via the RPC API");

                //stop mining, try the API first
                apiContext.QuitMining();
            }
        }
        finally
        {
            Console.WriteLine("Killing any remaining process");

            //then kill the process
            try
            {
                minerProcess.Kill();
                minerProcess.WaitForExit();
                minerProcess.Close();
            }
            catch (InvalidOperationException ex)
            {
                //already closed
            }
        }
    }
    else
    {
        Console.WriteLine("No devices capable of mining detected");
    }
}
finally
{
    Console.WriteLine("Cleaning up, deleting directory {0}", executablePath);
    Directory.Delete(executablePath, true);
}

Console.WriteLine("Press any key to exit");
Console.ReadKey();
```

### License

Copyright (C) 2013 Nathanial Woolls

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


[1]: https://github.com/ckolivas/cgminer
[2]: https://github.com/luke-jr/bfgminer
[3]: https://www.dropbox.com/s/ne5eywfx8v7hneb/MultiMiner-1.0.7.zip
[4]: http://ck.kolivas.org/apps/cgminer/
[5]: https://github.com/nwoolls/homebrew-xgminer
[6]: http://blog.nwoolls.com/2013/04/24/bitcoin-mining-on-mac-os-x-cgminer-bfgminer/
[7]: http://xquartz.macosforge.org/
[8]: http://www.mono-project.com/Main_Page
[9]: http://coinchoose.com/
[10]: http://luke.dashjr.org/programs/bitcoin/files/bfgminer/
[11]: https://www.dropbox.com/s/o08inghtw7ut1an/MultiMiner-1.0.7.exe
[12]: https://github.com/nwoolls/MultiMiner/releases
[13]: https://github.com/nwoolls/xgminer-osx/
[15]: http://talk.multiminerapp.com
[16]: https://github.com/nwoolls/MultiMiner/tree/master/MultiMiner.Xgminer
[17]: https://github.com/nwoolls/MultiMiner/tree/master/MultiMiner.Xgminer.Api
[18]: https://github.com/luke-jr/bfgminer/blob/bfgminer/README.RPC
[19]: https://github.com/nwoolls/MultiMiner/tree/master/MultiMiner.Example
[20]: http://www.silabs.com/products/mcu/pages/usbtouartbridgevcpdrivers.aspx
[21]: http://minecoin.net/bluefuryredfury-driver-for-windows/
[22]: http://www.ftdichip.com/Drivers/VCP.htm
[23]: http://zadig.akeo.ie/
[24]: http://support.amd.com/en-us/download
[25]: http://store.bitcoin.org.pl/support
