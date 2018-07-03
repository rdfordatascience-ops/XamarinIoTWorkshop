﻿using System;
using System.Threading.Tasks;

using Xamarin.Essentials;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinIoTWorkshop
{
    public class GeolocationViewModel : BaseViewModel
    {
        #region Fields
        Location _mostRecentLocation;
        ICommand _startGeolocationCommand, _sendMessage;
        #endregion

        #region Events
        public event EventHandler<Location> LocationUpdated;
        #endregion

        #region Properties
        ICommand StartGeolocationCommand => _startGeolocationCommand ??
            (_startGeolocationCommand = new Command(async () => await StartGeolocationDataCollection().ConfigureAwait(false)));

        ICommand SendMessage => _sendMessage ??
            (_sendMessage = new Command<Location>(async location => await IoTDeviceService.SendMessage(location).ConfigureAwait(false)));

        Location MostRecentLocation
        {
            get => _mostRecentLocation;
            set 
            {
                var milesTraveled = Location.CalculateDistance(_mostRecentLocation, value, DistanceUnits.Miles);

                if(milesTraveled > 0.5)
                {
                    _mostRecentLocation = value;
                    OnLocationUpdated(value);
                }
            }
        }
        #endregion

        #region Methods
        protected override void StartDataCollection()
        {
            base.StartDataCollection();

            StartGeolocationCommand?.Execute(null);
        }

        async Task StartGeolocationDataCollection()
        {
            do
            {
                try
                {
                    MostRecentLocation = await GeolocationService.GetLocation().ConfigureAwait(false);

                    SendMessage?.Execute(MostRecentLocation);

                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    DataCollectionButtonCommand?.Execute(null);
                }

            } while (IsDataCollectionActive);
        }

        void OnLocationUpdated(Location location) => LocationUpdated?.Invoke(this, location);
        #endregion
    }
}
