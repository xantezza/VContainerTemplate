using UnityEngine;
using VContainer;

namespace VContainerTemplate.Infrastructure.Settings
{
    public class SettingsAccessor : MonoBehaviour
    {
        private ISettingsService _settingsService;

        [Inject]
        private void Inject(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void MuteAudio()
        {
            _settingsService.MuteAudio();
        }

        public void UnMuteAudio()
        {
            _settingsService.UnMuteAudio();
        }

        public void SetMusicVolume(float value)
        {
            _settingsService.SetMusicVolume(value);
        }

        public void SetSFXVolume(float value)
        {
            _settingsService.SetSFXVolume(value);
        }
    }
}