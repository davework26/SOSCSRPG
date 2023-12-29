﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
    public class Player : LivingEntity
    {
        #region Properties

        private int _experiencePoints;

        public int ExperiencePoints
        {
            get => _experiencePoints;
            private set
            {
                _experiencePoints = value;
                OnPropertyChanged( );

                SetLevelAndMaximumHitPoints();
            }
        }

        public ObservableCollection<QuestStatus> Quests { get; } = 
            new ObservableCollection<QuestStatus>();

        public ObservableCollection<Recipe> Recipes { get; } = 
            new ObservableCollection<Recipe>();

        #endregion

        public event EventHandler OnLevelUp;

        public Player(string name, int experiencePoints,
                     int maximumHitPoints, int currentHitPoints, IEnumerable<PlayerAttribute> attributes, int gold) :
           base(name, maximumHitPoints, currentHitPoints, attributes, gold)
        {
            ExperiencePoints = experiencePoints;
        }

        public void AddExperience(int experiencePoints)
        {
            ExperiencePoints += experiencePoints;
        }


        public void LearnRecipe(Recipe recipe)
        {
            if (!Recipes.Any(r => r.ID == recipe.ID))
            {
                Recipes.Add(recipe);
            }
        }


        private void SetLevelAndMaximumHitPoints()                    
        {
            int originalLevel = Level;

            Level = (ExperiencePoints / 100) + 1;

            if (Level != originalLevel)
            {
                MaximumHitPoints = Level * 10;
                OnLevelUp?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }
}