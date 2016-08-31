using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UniversalBeaconLibrary.Beacon
{
    public class EstimoteNearableFrame : BeaconFrameBase
    {
        private string _id;
        private uint _major;
        private uint _minor;
        private string _uuid;
        private string _type;
        private float _tempterature;
        private bool _moving;
        private double _voltage;
        private double _xacc;
        private double _yacc;
        private double _zacc;
        private uint _currentMotionStateDuration;
        private uint _previousMotionStateDuration;



        public bool isMoving
        {
            get { return _moving; }
            set
            {
                if (_moving == value) return;
                _moving = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        public string Uuid
        {
            get { return _uuid; }
            set
            {
                if (_uuid == value) return;
                _uuid = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        public double Xacc
        {
            get { return _xacc; }
            set
            {
                if (_xacc == value) return;
                _xacc = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        public EstimoteNearableFrame(byte[] payload) : base(payload)
        {
            ParsePayload();
        }

        /// <summary>
        /// Parse the current payload into the properties exposed by this class.
        /// Has to be called if manually modifying the raw payload.
        /// </summary>
        public void ParsePayload()
        {
            using (var ms = new MemoryStream(Payload, false))
            {
                using (var reader = new BinaryReader(ms))
                {
                    ms.Position = 1;

                    var uuidBytes = reader.ReadBytes(8);
                    var newUuid = BitConverter.ToString(uuidBytes).Replace("-", "").ToLower();
                    if (Uuid != newUuid)
                    {
                        _uuid = newUuid;
                        OnPropertyChanged(nameof(Uuid));
                    }

                    ms.Position = 13;
                    bool newIsMoving = (reader.ReadByte() & 0x40) != 0;
                    if (isMoving != newIsMoving)
                    {
                        _moving = newIsMoving;
                        OnPropertyChanged(nameof(isMoving));
                    }

                    double newXacc = (int)reader.ReadByte() * 15.625;
                    if(newXacc != _xacc)
                    {
                        _xacc = newXacc;
                        OnPropertyChanged(nameof(Xacc));
                    }

    
                }
            }
        }

        /// <summary>
        /// Update the raw payload when properties have changed.
        /// </summary>
        private void UpdatePayload()
        {          
            return;
            using (var ms = new MemoryStream())
            {
               
            }
        }

        /// <summary>
        /// Update the information stored in this frame with the information from the other frame.
        /// Useful for example when binding the UI to beacon information, as this will emit
        /// property changed notifications whenever a value changes - which would not be possible if
        /// you would overwrite the whole frame.
        /// </summary>
        /// <param name="otherFrame">Frame to use as source for updating the information in this beacon
        /// frame.</param>
        public override void Update(BeaconFrameBase otherFrame)
        {
            base.Update(otherFrame);
            ParsePayload();
        }

        /// <summary>
        /// Check if the contents of this frame are generally valid.
        /// Does not currently perform a deep analysis, but checks the header as well
        /// as the frame length.
        /// </summary>
        /// <returns>True if the frame is a valid Eddystone TLM frame.</returns>
        public override bool IsValid()
        {
            return base.IsValid();
        }
    }
}
