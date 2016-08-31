// Copyright 2015 Andreas Jakl, Tieto Corporation. All rights reserved. 
// https://github.com/andijakl/universal-beacon 
// 
// Based on the Google Eddystone specification, 
// available under Apache License, Version 2.0 from
// https://github.com/google/eddystone
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
//    http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 

using System.Collections.ObjectModel;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.Advertisement;
using UniversalBeaconLibrary.Beacon;
using System;
using System.Diagnostics;
namespace UniversalBeaconLibrary.Beacon
{
    /// <summary>
    /// Manages multiple beacons and distributes received Bluetooth LE
    /// Advertisements based on unique Bluetooth beacons.
    /// 
    /// Whenever your app gets a callback that it has received a new Bluetooth LE
    /// advertisement, send it to the ReceivedAdvertisement() method of this class,
    /// which will handle the data and either add a new Bluetooth beacon to the list
    /// of beacons observed so far, or update a previously known beacon with the
    /// new advertisement information.
    /// </summary>
    public class BeaconManager
    {
        /// <summary>
        /// List of known beacons so far, which all have a unique Bluetooth MAC address
        /// and can have multiple data frames.
        /// </summary>
        public ObservableCollection<Beacon> BluetoothBeacons { get; set; } = new ObservableCollection<Beacon>();
        /// <summary>
        /// Analyze the received Bluetooth LE advertisement, and either add a new unique
        /// beacon to the list of known beacons, or update a previously known beacon
        /// with the new information.
        /// </summary>
        /// <param name="btAdv">Bluetooth advertisement to parse, as received from
        /// the Windows Bluetooth LE API.</param>
        public void ReceivedAdvertisement(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            if (btAdv == null) return;

            // Check if we already know this bluetooth address
            foreach (var bluetoothBeacon in BluetoothBeacons)
            {
                if (bluetoothBeacon.CheckAddress(btAdv))
                {
                    // We already know this beacon
                    // Update / Add info to existing beacon
                    bluetoothBeacon.UpdateBeacon(btAdv);
                    return;
                }
            }

            for(int i = BluetoothBeacons.Count - 1; i>=0; i--)
            {
                if ((DateTimeOffset.Now - BluetoothBeacons[i].Timestamp).Seconds > 10)
                {
                    BluetoothBeacons.Remove(BluetoothBeacons[i]);
                }
            }

            // Beacon was not yet known - add it to the list.
            var newBeacon = new Beacon(btAdv);
            Beacon.BeaconTypeEnum[] filter = { Beacon.BeaconTypeEnum.EstimoteStone, Beacon.BeaconTypeEnum.EstimoteNearable };
            if (Array.IndexOf(filter, newBeacon.BeaconType) > -1)
            {
                BluetoothBeacons.Add(newBeacon);
                if(newBeacon.BluetoothAddressAsString == "60202cd8293ba3d9")
                {
                    newBeacon.PropertyChanged += NewBeacon_PropertyChanged;
                }
            }
        }

        private void NewBeacon_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "isMoving")
            {
                Debug.WriteLine("moved");
            }
          
        }
    }
}
