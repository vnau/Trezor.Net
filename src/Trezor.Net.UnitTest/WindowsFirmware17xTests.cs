﻿using Hid.Net;
using LibUsbDotNet.LibUsb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Trezor.Net
{
    [TestClass]
    public class WindowsFirmware17xTests : UnitTestBase
    {
        #region Fields
        private UsbContext _UsbContext;
        private LibUsbDevice _LibUsbDevice;
        #endregion

        #region Implementation
        protected override async Task<IHidDevice> Connect()
        {
            _UsbContext = new UsbContext();

            var trezorUsbDevice = _UsbContext.List().FirstOrDefault
                (
                d =>
                //Trezor One - 1.6.x
                (d.ProductId == 0x1 && d.VendorId == 0x534C) ||
                //Trezor One - 1.7.x
                (d.ProductId == 0x53C1 && d.VendorId == 0x1209)
                );

            _LibUsbDevice = new LibUsbDevice(trezorUsbDevice, 64, 64, 60000);
            await _LibUsbDevice.InitializeAsync();

            Console.WriteLine("Connected");

            return _LibUsbDevice;
        }

        protected override async Task<string> GetPin()
        {
            var passwordExePath = Path.Combine(GetExecutingAssemblyDirectoryPath(), "Misc", "GetPassword.exe");
            if (!File.Exists(passwordExePath))
            {
                throw new Exception($"The pin exe doesn't exist at passwordExePath {passwordExePath}");
            }

            var process = Process.Start(passwordExePath);
            process.WaitForExit();
            await Task.Delay(100);
            var pin = File.ReadAllText(Path.Combine(GetExecutingAssemblyDirectoryPath(), "pin.txt"));
            return pin;
        }
        #endregion

        #region Setup / Tear Down
        [TestCleanup]
        public void Teardown()
        {
            TrezorManager?.Dispose();
            TrezorManager = null;
            _UsbContext?.Dispose();
        }
        #endregion

        #region Helpers
        private static string GetExecutingAssemblyDirectoryPath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var executingAssemblyDirectoryPath = Path.GetDirectoryName(uri.Path);
            return executingAssemblyDirectoryPath;
        }
        #endregion
    }
}