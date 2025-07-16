using System;
using System.Collections.Generic;
using DRMG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DRMG.Core
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField, Min(2)] private int maxGridLength = 2;
        [SerializeField] private Toggle allowRepititionToggle;
        [SerializeField] private TMP_Dropdown gridSizeDropdown;
        private List<GridSizeOption> gridSizeOptions = new List<GridSizeOption>();

        private void OnEnable()
        {
            RebuildGridSizeDropdown();
            gridSizeDropdown.onValueChanged.RemoveAllListeners();
            gridSizeDropdown.onValueChanged.AddListener(SetGridSize);
            allowRepititionToggle.onValueChanged.RemoveAllListeners();
            allowRepititionToggle.onValueChanged.AddListener(SetAllowRepititions);
            allowRepititionToggle.isOn = MatchDataManager.MatchDataSubject.AllowRepitition();
        }

        private void OnValidate()
        {
            for (int i = 0; i < gridSizeOptions.Count; i++)
            {
                GridSizeOption option = gridSizeOptions[i];
                if (option.GetSize() % 2 != 0)
                {
                    option.width = 2;
                    option.height = 2;
                    gridSizeOptions[i] = option;
                }
            }
        }

        private void SetAllowRepititions(bool allowRepititions)
        {
            MatchDataManager.MatchDataSubject.SetAllowRepititions(allowRepititions);
            RebuildGridSizeDropdown();
        }

        private void SetGridSize(int index)
        {
            if (index < 0 || index >= gridSizeOptions.Count)
                return;
            GridSizeOption option = gridSizeOptions[index];
            if (!MatchDataManager.MatchDataSubject.ValidGridSize(option.width, option.height)
                && !MatchDataManager.MatchDataSubject.AllowRepitition())
            {
                allowRepititionToggle.isOn = true;
                MatchDataManager.MatchDataSubject.SetAllowRepititions(true);
            }

            MatchDataManager.MatchDataSubject.SetGridSize(option.width, option.height);
        }

        private void RebuildGridSizeDropdown()
        {
            gridSizeDropdown.options.Clear();
            for (int x = 2; x <= maxGridLength; x++)
                for (int y = 2; y <= maxGridLength; y++)
                    if (MatchDataManager.MatchDataSubject.ValidGridSize(x, y))
                        gridSizeOptions.Add(new GridSizeOption(x, y));
            foreach (GridSizeOption option in gridSizeOptions)
                gridSizeDropdown.options.Add(new TMP_Dropdown.OptionData(option.GetText()));

            int targetSizeIndex = -1;
            for (int i = 0; i < gridSizeOptions.Count; i++)
            {
                GridSizeOption gridSizeOption = gridSizeOptions[i];
                if (gridSizeOption.width == MatchDataManager.MatchDataSubject.GetGridWidth()
                    && gridSizeOption.height == MatchDataManager.MatchDataSubject.GetGridHeight())
                {
                    targetSizeIndex = i;
                    break;
                }
            }

            if (targetSizeIndex == -1)
            {
                targetSizeIndex = 0;
                SetGridSize(0);
            }

            gridSizeDropdown.SetValueWithoutNotify(targetSizeIndex);
        }

        [Serializable]
        internal struct GridSizeOption
        {
            [Min(2)] public int width;
            [Min(2)] public int height;
            public int GetSize() => width * height;
            public string GetText() => $"{width}x{height}";

            public GridSizeOption(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
        }
    }
}