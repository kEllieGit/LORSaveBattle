using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SaveBattle.Data;
using HarmonyLib;

namespace SaveBattle
{
    /// <summary>
    /// Provides methods for interfacing with the saved battle data.
    /// </summary>
    public static class SavedBattle
    {
        /// <summary>
        /// Loads the saved battle data onto the currently active stage.
        /// </summary>
        public static async void Load()
        {
            try
            {
                if (!File.Exists(SerializationInfo.Filepath))
                {
                    throw new FileNotFoundException("Data file not found!");
                }

                var json = File.ReadAllText(SerializationInfo.Filepath);
                var saveData = JsonSerializer.Deserialize<StageSaveData>(json, SerializationInfo.GetOptions());

                if (saveData.GetStage() is StageClassInfo stage)
                {
                    var stageController = Singleton<StageController>.Instance;
                    switch (saveData.StageType)
                    {
                        default:
                            stageController.SetCurrentSephirah(saveData.Sephirah);
                            stageController.InitStageByInvitation(stage, null);
                            break;
                        case StageManagerType.Creature:
                            stageController.InitStageByCreature(stage);
                            break;
                        case StageManagerType.Enemy:
                            stageController.InitStageByEndContentsStage(stage);
                            break;
                        case StageManagerType.KetherFinal:
                            stageController.InitStageForKeterCompleteOpen(stage);
                            break;
                    }
                    stageController.SetCurrentWave(saveData.Wave);

                    UI.UIController.Instance.SetStageInfo(stage);
                    GlobalGameManager.Instance.LoadBattleScene();

                    await Task.Delay(2000);
                    LoadBattleData(saveData, stageController, stage);
                }
                else
                {
                    GameSceneManager.Instance.ActivateBattleScene();
                    SingletonBehavior<BattleSceneRoot>.Instance.StartBattle();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to load saved battle data! {ex}");
            }
        }

        private static void LoadBattleData(StageSaveData stageData, StageController stageController, StageClassInfo stage)
        {
            try
            {
                var turnField = AccessTools.Field(typeof(StageController), "_roundTurn");
                turnField.SetValue(stageController, stageData.Turn);

                var usedFloorsField = AccessTools.Field(typeof(StageController), "_usedFloorList");
                usedFloorsField.SetValue(stageController, stageData.UsedFloors);

                var stageFloor = stageController.GetCurrentStageFloorModel();
                var team = stageFloor.team;
                team.currentSelectEmotionLevel = stageData.EmotionLevel;

                var emotionLevelField = AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionLevel");
                emotionLevelField.SetValue(team, stageData.EmotionLevel);

                var egoCardIds = stageData.EGOCards.Select(card => card.GetLorId()).ToList();
                foreach (var egoCard in egoCardIds)
                {
                    Singleton<SpecialCardListModel>.Instance.AddCard(egoCard, stageData.Sephirah);
                }

                var units = BattleObjectManager.instance.GetAliveList();

                var aliveUnitIndexes = new HashSet<int>(units.Select(unit => unit.index));
                var unmatchedUnitData = stageData.UnitData
                    .Where(x => !aliveUnitIndexes.Contains(x.Index))
                    .ToList();

                foreach (var aliveUnit in units)
                {
                    var unitData = stageData.UnitData.Where(x => x.Index == aliveUnit.index).FirstOrDefault();
                    if (unitData == null)
                    {
                        BattleObjectManager.instance.UnregisterUnit(aliveUnit);
                    }
                }

                LoadUnitData(units, stageData);

                var factionPlayerUnits = unmatchedUnitData
                    .Where(x => x.Faction == Faction.Player)
                    .ToList();

                var factionEnemyUnits = unmatchedUnitData
                    .Where(x => x.Faction == Faction.Enemy)
                    .ToList();

                CreateMissingUnits(factionPlayerUnits, stageData);
                CreateMissingUnits(factionEnemyUnits, stageData);
                BattleObjectManager.instance.InitUI();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to load saved battle data! {ex}");
            }
        }

        private static void LoadUnitData(IEnumerable<BattleUnitModel> units, StageSaveData stageData)
        {
            foreach (var unit in units)
            {
                var aliveUnitData = unit.UnitData.unitData;

                var unitSaveData = stageData.UnitData
                    .FirstOrDefault(x => x.Index == unit.index && x.Faction == unit.faction);

                if (unitSaveData != null)
                {
                    unit.LoadFromSaveData(unitSaveData, stageData);
                }
            }
        }

        private static void CreateMissingUnits(IEnumerable<UnitSaveData> unitData, StageSaveData stageData)
        {
            var stageController = Singleton<StageController>.Instance;

            foreach (var unmatchedUnit in unitData)
            {
                var aliveUnits = BattleObjectManager.instance.GetAliveList(unmatchedUnit.Faction);
                if (aliveUnits.Any(x => x.index == unmatchedUnit.Index))
                {
                    continue;
                }

                BattleUnitModel newUnit = null;

                switch (unmatchedUnit.Faction)
                {
                    case Faction.Enemy:
                        newUnit = stageController.AddNewUnit(Faction.Enemy, unmatchedUnit.ID.GetLorId(), unmatchedUnit.Index);
                        break;
                    case Faction.Player:
                        newUnit = stageController.CreateLibrarianUnit_fromBattleUnitData(unmatchedUnit.Index);
                        break;
                }

                newUnit.LoadFromSaveData(unmatchedUnit, stageData);
            }
        }

        /// <summary>
        /// Saves the current active battle data.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static async void Save(StageManagerType managerType = StageManagerType.Map)
        {
            // Wait a split second for anything we might miss.
            await Task.Delay(2000);

            try
            {
                var stageController = Singleton<StageController>.Instance;
                if (stageController == null)
                {
                    throw new NullReferenceException("Unable to save battle data, StageController was null!");
                }

                var stageFloor = stageController.GetCurrentStageFloorModel();
                var stageModelInfo = stageController.GetStageModel().ClassInfo;
                var stageSaveData = new StageSaveData
                {
                    StageID = new SaveId(stageModelInfo.id),
                    StageType = managerType,
                    Wave = stageController.CurrentWave,
                    Turn = stageController.RoundTurn,
                    EmotionLevel = stageFloor.team.emotionLevel,
                    Sephirah = stageController.CurrentFloor,
                    UsedFloors = stageController.usedFloorList,
                };
                stageSaveData.EGOCards.AddRange(Singleton<SpecialCardListModel>.Instance.GetHand().Select(card => new SaveId(card.GetID())));

                var Units = BattleObjectManager.instance.GetAliveList();
                foreach (var unit in Units)
                {
                    var unitData = unit.UnitData.unitData;
                    var unitID = unitData.EnemyUnitId;

                    var unitSaveData = new UnitSaveData
                    {
                        Name = unitData.name,
                        ID = new SaveId(unitID),
                        Index = unit.index,
                        HP = unit.hp,
                        SR = unit.breakDetail.breakGauge,
                        IsStaggered = unit.breakDetail.IsBreakLifeZero(),
                        PlayPoints = unit.PlayPoint,
                        EmotionLevel = unit.emotionDetail.EmotionLevel,
                        Faction = unit.faction,
                    };

                    unitSaveData.Hand.AddRange(unit.allyCardDetail.GetHand().Select(card => new SaveId(card.GetID())));
                    unitSaveData.EGOHand.AddRange(unit.personalEgoDetail.GetHand().Select(card => new SaveId(card.GetID())));
                    unitSaveData.EmotionCards.AddRange(unit.emotionDetail.PassiveList.Select(card => card.XmlInfo.id));
                    unitSaveData.EmotionCoins.AddRange(unit.emotionDetail.AllEmotionCoins.Select(coin => new EmotionCoinData(coin.CoinType)));
                    unitSaveData.Passives.AddRange(unit.passiveDetail.PassiveList.Select(entry => new SaveId(entry.id)));

                    var bufs = unit.bufListDetail.GetActivatedBufList().Concat(unit.bufListDetail.GetReadyBufList());
                    foreach (var buf in bufs)
                    {
                        var bufName = buf.GetType().Name;
                        if (unitSaveData.Bufs.ContainsKey(bufName))
                        {
                            unitSaveData.Bufs[bufName] = buf.stack;
                        }
                        else
                        {
                            unitSaveData.Bufs.Add(bufName, buf.stack);
                        }
                    }

                    stageSaveData.UnitData.Add(unitSaveData);
                }

                using (FileStream stream = File.Create(SerializationInfo.Filepath))
                {
                    JsonSerializer.Serialize(stream, stageSaveData, SerializationInfo.GetOptions());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to save battle data! {ex}");
            }
        }
    }
}
