using System;
using System.IO;
using System.Collections.Generic;
using Engine.Factories;
using Engine.Models;
using Engine.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Engine.Services
{
    public static class SaveGameService
    {
        public static void Save(GameSession gameSession, string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(gameSession, Formatting.Indented));
        }

        public static GameSession LoadLastSaveOrCreateNew(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"Filename: {filename}");
            }

            // Save game file exists so create GameSession from it.
            try
            {
                JObject data = JObject.Parse(File.ReadAllText(filename));

                Player player = CreatePlayer(data);

                int x = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.XCoordinate)];
                int y = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.YCoordinate)];

                return new GameSession(player, x, y);
            }
            catch (Exception ex)
            {
                // If there was an error loading/deserialising the saved game,
                // throw exception.
                throw new FormatException($"Error reading: {filename}");
            }
        }

        private static Player CreatePlayer(JObject data)
        {
            //string fileVersion = FileVersion(data);

            Player player;

            //switch (fileVersion)
            //{
            //    case "0.1.000":
                    player =
                        new Player((string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Name)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.ExperiencePoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.MaximumHitPoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.CurrentHitPoints)],
                                   GetPlayerAttributes(data),
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Gold)]);
            //        break;
            //    default:
            //        throw new InvalidDataException($"File version '{fileVersion}' not recognised");
            //}

            PopulatePlayerInventory(data, player);
            PopulatePlayerQuests(data, player);
            PopulatePlayerRecipes(data, player);

            return player;
        }

        public static IEnumerable<PlayerAttribute> GetPlayerAttributes(JObject data)
        {
            List<PlayerAttribute> attributes =
                new List<PlayerAttribute>();

            foreach(JToken itemToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                [nameof(Player.Attributes)])
            {
                attributes.Add(new PlayerAttribute(
                                (string)itemToken[nameof(PlayerAttribute.Key)],
                                (string)itemToken[nameof(PlayerAttribute.DisplayName)],
                                (string)itemToken[nameof(PlayerAttribute.DiceNotation)],
                                (int)itemToken[nameof(PlayerAttribute.BaseValue)],
                                (int)itemToken[nameof(PlayerAttribute.ModifiedValue)]));
            }

            return attributes;
        }

        private static void PopulatePlayerInventory(JObject data, Player player)
        {
            //string fileVersion = FileVersion(data);

            //switch (fileVersion)
            //{
            //    case "0.1.000":
                    foreach (JToken itemToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                                                            [nameof(Player.Inventory)]
                                                            [nameof(Inventory.Items)])
                    {
                        int itemId = (int)itemToken[nameof(GameItem.ItemTypeID)];
                        player.AddItemToInventory(ItemFactory.CreateGameItem(itemId));
                    }
            //        break;
            //    default:
            //        throw new InvalidDataException($"File version '{fileVersion}' not recognised");
            //}
        }

        private static void PopulatePlayerQuests(JObject data, Player player)
        {
            //string fileVersion = FileVersion(data);

            //switch (fileVersion)
            //{
            //    case "0.1.000":
                    foreach (JToken questToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Quests)])
                    {
                        int questId =
                            (int)questToken[nameof(QuestStatus.PlayerQuest)][nameof(QuestStatus.PlayerQuest.ID)];

                        Quest quest = QuestFactory.GetQuestByID(questId);
                        QuestStatus questStatus = new QuestStatus(quest);
                        questStatus.IsCompleted = (bool)questToken[nameof(QuestStatus.IsCompleted)];

                        player.Quests.Add(questStatus);
                    }

            //        break;
            //    default:
            //        throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            //}
        }

        private static void PopulatePlayerRecipes(JObject data, Player player)
        {
            //string fileVersion = FileVersion(data);

            //switch (fileVersion)
            //{
            //    case "0.1.000":
                    foreach (JToken recipeToken in
                        (JArray)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Recipes)])
                    {
                        int recipeId = (int)recipeToken[nameof(Recipe.ID)];

                        Recipe recipe = RecipeFactory.RecipeByID(recipeId);

                        player.Recipes.Add(recipe);
                    }

            //        break;
            //    default:
            //        throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            //}
        }

        //private static string FileVersion(JObject data)
        //{
        //    return (string)data[nameof(GameSession.GameDetails .Version)];
        //}    
    }
}
