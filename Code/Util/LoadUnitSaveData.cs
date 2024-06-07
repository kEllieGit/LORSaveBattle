using System;
using System.Reflection;
using System.Linq;
using SaveBattle.Data;

namespace SaveBattle
{
    public static class BattleUnitModelExtensions
    {
        public static void LoadFromSaveData(this BattleUnitModel unit, UnitSaveData unitData, StageSaveData stageData)
        {
            try
            {
                unit.SetHp((int)unitData.HP);
                unit.breakDetail.breakGauge = (int)unitData.SR;
                unit.breakDetail.breakLife = unitData.IsStaggered ? 0 : 1;
                unit.cardSlotDetail.SetPlayPoint(unitData.PlayPoints);

                var unitPassives = unit.passiveDetail.PassiveList;
                //unit.passiveDetail.DestroyPassiveAll();
                foreach (var passive in unitData.Passives)
                {
                    if (!unitPassives.Any(x => x.id == passive))
                    {
                        unit.passiveDetail.AddPassive(passive.GetLorId());
                    }
                }

                unit.SetEmotionData(unitData, stageData);
                unit.AddCards(unitData, stageData);
                unit.SetBufData(unitData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to load saved unit data! {ex}");
            }
        }

        private static void SetBufData(this BattleUnitModel unit, UnitSaveData unitData)
        {
            try
            {
                //unit.bufListDetail.GetActivatedBufList().Clear();
                //unit.bufListDetail.GetReadyBufList().Clear();
                foreach (var buf in unitData.Bufs.Keys)
                {
                    // Currently can not find nested bufs within PassiveAbility types.
                    var bufType = Type.GetType(buf) ?? AssemblyHelper.GetTypeFromLoadedAssemblies(buf);

                    if (bufType == null)
                    {
                        continue;
                    }

                    if (Activator.CreateInstance(bufType) is BattleUnitBuf bufInstance)
                    {
                        unit.bufListDetail.AddBuf(bufInstance);
                        bufInstance.stack = unitData.Bufs[buf];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to load saved unit data! {ex}");
            }
        }

        private static void SetEmotionData(this BattleUnitModel unit, UnitSaveData unitData, StageSaveData stageData)
        {
            try
            {
                unit.emotionDetail.SetEmotionLevel(unitData.EmotionLevel);
                unit.emotionDetail.AllEmotionCoins.Clear();
                foreach (var coin in unitData.EmotionCoins)
                {
                    ConstructorInfo ci = typeof(EmotionCoin).GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new Type[] { typeof(EmotionCoinType) }, null);

                    object[] param = new object[] { coin.CoinType };
                    EmotionCoin co = (EmotionCoin)ci.Invoke(param);

                    unit.emotionDetail.AllEmotionCoins.Add(co);
                }

                if (unit.faction == Faction.Player)
                {
                    unit.emotionDetail.RemoveAllEmotionCard();
                    foreach (var emotionCardId in unitData.EmotionCards)
                    {
                        var card = EmotionCardXmlList.Instance.GetData(emotionCardId, stageData.Sephirah);
                        unit.emotionDetail.ApplyEmotionCard(card);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to load saved unit data! {ex}");
            }
        }

        private static void AddCards(this BattleUnitModel unit, UnitSaveData unitData, StageSaveData stageData)
        {
            try
            {
                unit.allyCardDetail.ExhaustAllCardsInHand();
                foreach (var cardId in unitData.Hand)
                {
                    unit.allyCardDetail.AddNewCard(cardId.GetLorId());
                    unit.allyCardDetail.ExhaustCardInDeck(cardId.GetLorId());
                }

                unit.personalEgoDetail.GetCardAll().Clear();
                foreach (var egoCardId in unitData.EGOHand)
                {
                    unit.personalEgoDetail.AddCard(egoCardId.GetLorId());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Debug.GetCurrentMethodName()}] Failed to load saved unit data! {ex}");
            }
        }
    }
}
