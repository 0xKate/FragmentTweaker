using Nautilus.Json.ExtensionMethods;
using Nautilus.Options;
using System;
using System.Collections.Generic;

namespace FragmentTweaker
{
    public class Settings : ModOptions
    {
        /// The base ModOptions class takes a string name as an argument
        public Settings() : base("Fragment Tweaker")
        {
            // Iterate over the TweakedTechs dictionary to create sliders dynamically
            foreach (KeyValuePair<TechType, int> tech in Plugin.TweakedTechs)
            {
                var techType = tech.Key;
                var fragmentCount = tech.Value;

                // Create a slider for this TechType
                var slider = ModSliderOption.Create(
                    TechTypeExtensions.EncodeKey(techType), // Unique ID
                    techType.ToString(),       // Display name
                    0,                         // Min value
                    20,                        // Max value
                    fragmentCount,             // Default value
                    fragmentCount,             // Initial value
                    "{0}"                      // Format string for integer display
                );

                // Subscribe to the OnChanged event
                slider.OnChanged += OnFragmentChange;

                // Add the slider to the options menu
                AddItem(slider);
            }
        }

        /// Handles slider value changes
        public void OnFragmentChange(object sender, SliderChangedEventArgs e)
        {
            try
            {
                // Decode the TechType from the slider's ID
                TechType techType = TechTypeExtensions.DecodeKey(e.Id);

                // Update the fragment count in the dictionary
                if (Plugin.TweakedTechs.ContainsKey(techType))
                    Plugin.TweakedTechs[techType] = (int)e.Value;

                // Save the updated dictionary to JSON
                Plugin.TweakedTechs.SaveJson(Plugin.TechDB);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to update fragment count: {ex.Message}");
            }
        }
    }
}
