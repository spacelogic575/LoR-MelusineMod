using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using LOR_DiceSystem;
using Sound;
using BaseMod;
using static PassiveAbility_250223;
using static UnityEngine.GraphicsBuffer;
using UI;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace Meluko
{
    public class bufAdder
    {

        public static void addMomentum(BattleUnitModel target, int stack, BattleUnitModel owner)
        {
            BattleUnitBuf tempbuf = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum);
            if (tempbuf != null)
            {
                tempbuf.OnAddBuf(stack);
                tempbuf.stack += stack;
            }
            else
            {
                tempbuf.OnAddBuf(stack);
                target.bufListDetail.AddBuf(new BattleUnitBuf_Momentum
                {
                    stack = stack
                });
            }
        }

        public static bool removeMomentum(BattleUnitModel target, int stack, BattleUnitModel owner)
        {
            BattleUnitBuf tempbuf = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum);
            if (tempbuf != null)
            {
                if (tempbuf.stack >= stack)
                {
                    tempbuf.OnAddBuf(stack);
                    tempbuf.stack -= stack;
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            return false;
        }

        public static void addMark(BattleUnitModel target)
        {  
                target.bufListDetail.AddBuf(new BattleUnitBuf_AzureMark
                {
                    stack = 1
                });          
        }

        public static void removeMark(BattleUnitModel target)
        {
            BattleUnitBuf tempbuf = target.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_AzureMark);
            if (tempbuf != null)
            {
                target.bufListDetail.RemoveBuf(new BattleUnitBuf_AzureMark
                {
                    stack = 1
                });
            }
        }

    }

    public class BattleUnitBuf_AzureMark : BattleUnitBuf
    {
        protected override string keywordId => "AzureMark";
        protected override string keywordIconId => "AzureMark";
        public override bool independentBufIcon => false;

        // Token: 0x0600007D RID: 125 RVA: 0x00002367 File Offset: 0x00000567
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            this.timer = true;
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00002376 File Offset: 0x00000576
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            if (this.timer)
            {
                this.Destroy();
            }
        }

        // Token: 0x0400004A RID: 74
        public bool timer;
    }

    public class BattleUnitBuf_Momentum : BattleUnitBuf
    {
        protected override string keywordId => "Momentum";
        protected override string keywordIconId => "Momentum";
        public override bool independentBufIcon => false;
        public override void OnAddBuf(int addedStack)
        {
            bool flag = this.IsValidUser();
            if (flag)
            {
                if (this.stack > 30)
                {
                    this.stack = 30;
                }
                if (this.stack < 0)
                {
                    this.stack = 0;
                }
            }
            if (!flag && this.stack > 0)
            {
                this.stack = 0;
            }
        }

        private bool IsValidUser()
        {
            return this._owner.passiveDetail.HasPassive<PassiveAbility_PlayerMelukoPerryDancer>();
        }
    }

    public class DiceCardAbility_AvoidanceInstincts : DiceCardAbilityBase
    {
        public override bool IsImmuneDestory
        {
            get
            {
                return true;
            }
        }

        public static string Desc = "This dice cannot be destroyed.";
    }

    public class DiceCardAbility_InnocenceArondightDice : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            target.TakeDamage(10, DamageType.Card_Ability, base.owner, KeywordBuf.None);
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 2, base.owner);
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable_break, 2, base.owner);
            target?.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Vulnerable, 2, base.owner);
            target?.bufListDetail.AddKeywordBufNextNextByCard(KeywordBuf.Vulnerable_break, 2, base.owner);
        }

        // Token: 0x0400004B RID: 75
        public static string Desc = "[On Hit] Deal 10 extra damage; Inflict 2 Fragile and Stagger Fragile for the next two Scenes.";
    }

    public class DiceCardAbility_HollowHeartAlbionDice : DiceCardAbilityBase
    {
        public override void OnSucceedAttack(BattleUnitModel target)
        {
            target.TakeDamage(5, DamageType.Card_Ability, base.owner, KeywordBuf.None);
            target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 5, base.owner);
        }

        // Token: 0x0400004B RID: 75
        public static string Desc = "[On Hit] Deal 5 extra damage and inflict 5 Burn.";
    }

    public class DiceCardSelfAbility_RayHorizon : DiceCardSelfAbilityBase
    {
        // Token: 0x0600004F RID: 79 RVA: 0x0000226B File Offset: 0x0000046B
        public IEnumerator Transform()
        {
            yield return YieldCache.WaitForSeconds(0.65f);
            base.owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect("Philip/Philip_Aura_Start", 1f, base.owner.view, base.owner.view, 1f);
            SoundEffectPlayer.PlaySound("Creature/MatchGirl_Explosion");
            yield return YieldCache.WaitForSeconds(0.8f);
            SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.SetRunningState(false);
            yield break;
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00002E58 File Offset: 0x00001058
        public static void Activate(BattleUnitModel unit)
        {
            unit.RecoverHP(40);
            unit.breakDetail.RecoverBreak(40);
            unit.cardSlotDetail.RecoverPlayPoint(10);
            PassiveAbilityBase passiveAbilityBase = unit.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_PlayerLakeLight);
            if (passiveAbilityBase != null)
            {
                unit.passiveDetail.DestroyPassive(passiveAbilityBase);
            }
            PassiveAbilityBase passiveAbilityBase2 = unit.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_PlayerCalamityOfFlames);
            if (passiveAbilityBase2 == null && unit.passiveDetail.AddPassive(new LorId("Meluko", 5)) != null && unit.passiveDetail.ReadyPassiveList.Contains(passiveAbilityBase2))
            {
                unit.passiveDetail.ReadyPassiveList.Remove(passiveAbilityBase2);
                unit.passiveDetail.PassiveList.Add(passiveAbilityBase2);
            }
            unit.personalEgoDetail.RemoveCard(new LorId("Meluko", 1));
            unit.personalEgoDetail.AddCard(new LorId("Meluko", 2));
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00002F74 File Offset: 0x00001174
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            DiceCardSelfAbility_RayHorizon.Activate(unit);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            self.exhaust = true;
            SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect("1/MatchGirl_Footfall", 1f, unit.view, unit.view, 2f).AttachEffectLayer();
            SoundEffectPlayer.PlaySound("Creature/MatchGirl_Explosion");
        }

        // Token: 0x04000033 RID: 51
        public static string Desc = "[On Play] Restore 40 HP and Stagger resist; Fully restore Light and transform into your second form.";
    }

    public class MeluluInitializer : ModInitializer
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void OnInitializeMod()
        {
            MeluluInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            MeluluInitializer.GetArtWorks(new DirectoryInfo(MeluluInitializer.path + "/ArtWork"));
        }
        public static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    MeluluInitializer.GetArtWorks(directories[i]);
                }
            }
            foreach (System.IO.FileInfo fileInfo in dir.GetFiles())
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                MeluluInitializer.ArtWorks[fileNameWithoutExtension] = value;
            }
        }
        public static string path;
        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
    } 
    

    public class PassiveAbility_PlayerCalamityOfFlames : PassiveAbilityBase
    {
        // Token: 0x0600004C RID: 76 RVA: 0x00002D28 File Offset: 0x00000F28
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (base.IsAttackDice(behavior.Detail))
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1
                });
                base.BeforeRollDice(behavior);
                BattleUnitModel target = behavior.card.target;
                if (base.IsAttackDice(behavior.Detail) && behavior != null && target != null && target.bufListDetail.GetKewordBufAllStack(KeywordBuf.Burn) > 0)
                {
                    behavior.ApplyDiceStatBonus(new DiceStatBonus
                    {
                        power = 2
                    });
                }
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x0000222D File Offset: 0x0000042D
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            base.OnSucceedAttack(behavior);
            (behavior?.card?.target)?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, RandomUtil.Range(2, 3), owner);
        }

        // Token: 0x0600004E RID: 78
        public override void OnRoundStart()
        {
            if (this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum) != null)
            {
                this.speedy = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum).stack / 10;
                if (this.speedy > 3)
                {
                    this.speedy = 3;
                }
                if (this.speedy >= 1)
                {
                    for (int i = 0; i < this.speedy; i++)
                    {
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, this.owner);
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 1, this.owner);
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1, this.owner);
                        this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DmgUp, 1, this.owner);
                    }
                }
            }
        }

        // Token: 0x0400002F RID: 47
        public static string Name = "Calamity of Flames";

        // Token: 0x04000030 RID: 48
        public static string Desc = "Add Hollow Heart Albion to the EGO hand. All attacks inflict 2-3 Burn on hit. All offensive dice gain +1 power, and gain +2 more power against targets with inflicted with Burn. For every 10 stacks of {Momentum}, gain 1 Strength, Protection, Stagger Protection, and Damage Up.";

        // Token: 0x04000031 RID: 49
        private bool burningTarget;

        // Token: 0x04000032 RID: 50
        private int speedy;
    }

    public class PassiveAbility_PlayerLakeLight : PassiveAbilityBase
    {
        // Token: 0x06000046 RID: 70 RVA: 0x00002ACC File Offset: 0x00000CCC
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            BattleCardTotalResult battleCardResultLog = this.owner.battleCardResultLog;
            if (battleCardResultLog != null)
            {
                battleCardResultLog.SetPassiveAbility(this);
            }
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1
            });
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00002205 File Offset: 0x00000405
        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Evasion)
            {
                behavior.isBonusEvasion = true;
            }
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00002B04 File Offset: 0x00000D04
        public override void OnDrawParrying(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Evasion && behavior.TargetDice.Detail == BehaviourDetail.Evasion)
            {
                this.owner.history.parryingDrawCount--;
                if (behavior.DiceResultValue > behavior.TargetDice.DiceResultValue)
                {
                    this.owner.passiveDetail.OnWinParrying(behavior);
                    this.owner.emotionDetail.OnWinParrying(behavior);
                    this.owner.bufListDetail.OnWinParrying(behavior);
                    this.owner.allyCardDetail.OnWinParrying(behavior);
                    this.owner.history.parryingWinCount++;
                }
                if (behavior.DiceResultValue < behavior.TargetDice.DiceResultValue)
                {
                    this.owner.passiveDetail.OnLoseParrying(behavior);
                    this.owner.emotionDetail.OnLoseParrying(behavior);
                    this.owner.bufListDetail.OnLoseParrying(behavior);
                    this.owner.allyCardDetail.OnLoseParrying(behavior);
                    this.owner.history.parryingLoseCount++;
                }
            }
        }

        // Token: 0x06000049 RID: 73
        public override void OnStartBattle()
        {
            if (this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum) != null)
            {
                this.speedy = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum).stack / 10;
                if (this.speedy > 3)
                {
                    this.speedy = 3;
                }
                if (this.speedy >= 1)
                {
                    for (int i = 0; i < this.speedy; i++)
                    {
                        DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("Meluko", 4), false);
                        new DiceBehaviour();
                        List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
                        int num = 0;
                        foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
                        {
                            BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                            battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                            battleDiceBehavior.SetIndex(num++);
                            list.Add(battleDiceBehavior);
                        }
                        this.owner.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
                    }
                }
            }
        }

        // Token: 0x0400002C RID: 44
        public static string Name = "Innocent Lake's Light";

        // Token: 0x0400002D RID: 45
        public static string Desc = "Add Innocence Arondight to the EGO hand. All dice gain +1 power. For every 10 stacks of {Momentum}, gain an unbreakable counter dice (Evade, 4-8) at Combat Start. Evade dice are retained when clashing against defensive dice.";

        // Token: 0x0400002E RID: 46
        private int speedy;
    }

    public class PassiveAbility_PlayerMelukoAlbionRemnant : PassiveAbilityBase
    {
        // Token: 0x0600000B RID: 11 RVA: 0x00002408 File Offset: 0x00000608
        public override void OnWaveStart()
        {
            this.ascension = 1;
            this.transform = false;
            this.guts = false;
            this.owner.personalEgoDetail.AddCard(new LorId("Meluko", 1));
            this.owner.personalEgoDetail.AddCard(new LorId("Meluko", 3));
            PassiveAbilityBase passiveAbilityBase = this.owner.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_PlayerCalamityOfFlames);
            if (passiveAbilityBase != null)
            {
                this.owner.passiveDetail.DestroyPassive(passiveAbilityBase);
            }
            PassiveAbilityBase passiveAbilityBase2 = this.owner.passiveDetail.AddPassive(new LorId("Meluko", 4));
            if (passiveAbilityBase2 != null && this.owner.passiveDetail.ReadyPassiveList.Contains(passiveAbilityBase2))
            {
                this.owner.passiveDetail.ReadyPassiveList.Remove(passiveAbilityBase2);
                this.owner.passiveDetail.PassiveList.Add(passiveAbilityBase2);
            }
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002510 File Offset: 0x00000710
        public override int GetMinHp()
        {
            int result;
            if (!this.guts)
            {
                result = 1;
            }
            else if (this.guts)
            {
                result = 0;
            }
            else
            {
                result = base.GetMinHp();
            }
            return result;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000020B2 File Offset: 0x000002B2
        public IEnumerator Transform()
        {
            yield return YieldCache.WaitForSeconds(0.65f);
            this.owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect("Philip/Philip_Aura_Start", 1f, this.owner.view, this.owner.view, 1f);
            SoundEffectPlayer.PlaySound("Battle/Xiao_Vert");
            yield return YieldCache.WaitForSeconds(0.8f);
            SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.SetRunningState(false);
            yield break;
        }

        public override void OnRoundStart()
        {
            if (this.owner.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_PlayerLakeLight) == null) 
            {
                this.ascension = 2;
            }
            if (this.owner.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_PlayerCalamityOfFlames) != null)
            {
                this.transform = true;
            }
            if (this.owner.hp == 1f)
            {
                this.guts = true;
            }
            if (this.guts)
            {
                this.owner.view.StartCoroutine(this.Transform());
                if (this.owner.hp == 1f && this.ascension == 1)
                {
                    this.owner.RecoverHP(40);
                    if (!this.owner.breakDetail.IsBreakLifeZero())
                    {
                        this.owner.breakDetail.RecoverBreak((int)((float)this.owner.breakDetail.GetDefaultBreakGauge()));
                    }
                    if (this.owner.breakDetail.IsBreakLifeZero())
                    {
                        this.owner.breakDetail.RecoverBreak(this.owner.breakDetail.GetDefaultBreakGauge());
                        this.owner.breakDetail.nextTurnBreak = false;
                    }
                    this.ascension = 2;
                }
                else if (this.owner.hp == 1f && this.ascension == 2)
                {
                    this.owner.RecoverHP(20);
                    if (!this.owner.breakDetail.IsBreakLifeZero())
                    {
                        this.owner.breakDetail.RecoverBreak((int)((float)this.owner.breakDetail.GetDefaultBreakGauge()));
                    }
                    if (this.owner.breakDetail.IsBreakLifeZero())
                    {
                        this.owner.breakDetail.RecoverBreak(this.owner.breakDetail.GetDefaultBreakGauge());
                        this.owner.breakDetail.nextTurnBreak = false;
                    }
                    this.owner.cardSlotDetail.RecoverPlayPoint(10);
                }
            }
            bool flag = this.guts;
            if (this.ascension == 2 && !this.transform)
            {
                this.transform = true;
                this.owner.personalEgoDetail.RemoveCard(new LorId("Meluko", 1));
                this.owner.personalEgoDetail.RemoveCard(new LorId("Meluko", 3));
                this.owner.personalEgoDetail.AddCard(new LorId("Meluko", 2));
                PassiveAbilityBase passiveAbilityBase = this.owner.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_PlayerLakeLight);
                if (passiveAbilityBase != null)
                {
                    this.owner.passiveDetail.DestroyPassive(passiveAbilityBase);
                }
                PassiveAbilityBase passiveAbilityBase2 = this.owner.passiveDetail.AddPassive(new LorId("Meluko", 5));
                if (passiveAbilityBase2 != null && this.owner.passiveDetail.ReadyPassiveList.Contains(passiveAbilityBase2))
                {
                    this.owner.passiveDetail.ReadyPassiveList.Remove(passiveAbilityBase2);
                    this.owner.passiveDetail.PassiveList.Add(passiveAbilityBase2);
                }
            }
        }

        // Token: 0x04000003 RID: 3
        public static string Name = "Melusine - Remnant of Albion";

        // Token: 0x04000004 RID: 4
        public static string Desc = "This character possesses 2 decks, which are treated as separate forms and confers various buffs. Add Ray Horizon to the EGO hand that can be used to enter the second form. Upon taking lethal damage in the first form, prevent HP from falling below 1 for the Scene, and then forcibly recover from stagger and transform yourself into the second form with 40 HP the next Scene. Otherwise, restore 20 HP, recover from Stagger and refill all light at the start of the next Scene. This death prevention is shared across both forms and only activates once per Act.";

        // Token: 0x04000005 RID: 5
        private int ascension;

        // Token: 0x04000006 RID: 6
        private bool guts;

        // Token: 0x04000007 RID: 7
        public bool transform;
    }

    public class PassiveAbility_PlayerMelukoDragonHeart : PassiveAbilityBase
    {
        // Token: 0x06000005 RID: 5 RVA: 0x00002078 File Offset: 0x00000278
        public override int GetDamageReductionAll()
        {
            return RandomUtil.Range(2, 3);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002078 File Offset: 0x00000278
        public override int GetBreakDamageReductionAll(int dmg, DamageType dmgType, BattleUnitModel attacker)
        {
            return RandomUtil.Range(2, 3);
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000023A4 File Offset: 0x000005A4
        public override void OnRoundEnd()
        {
            if (this.owner.emotionDetail.EmotionLevel >= 3)
            {
                this.owner.cardSlotDetail.RecoverPlayPoint(1);
                this.owner.allyCardDetail.DrawCards(1);
            }
        }

        public override void OnRoundStart()
        {
            this.owner.RecoverHP(10);
            this.owner.breakDetail.RecoverBreak(5);
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002081 File Offset: 0x00000281
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                min = 1,
                max = 1
            });
        }

        // Token: 0x060000B3 RID: 179
        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            return AtkResist.Normal;
        }

        // Token: 0x060000B4 RID: 180
        public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
        {
            return AtkResist.Normal;
        }

        // Token: 0x04000001 RID: 1
        public static string Name = "Dragon Heart";

        // Token: 0x04000002 RID: 2
        public static string Desc = "Regenerate 10 HP and 5 Stagger resist per Scene. All dice gain +1 Minimum and Maximum roll value. Take 2-3 less damage and stagger damage (Including damage from non-attacks). At emotion level 3+, gain 1 extra light and draw 1 additional page at the start of each Scene.";
    }

    public class PassiveAbility_PlayerMelukoPerryDancer : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            bufAdder.addMomentum(this.owner, 10, this.owner); 
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.target != null && curCard.target.faction != owner.faction)
            {
                nextTarget = curCard.target;
            }
        }

        public override void OnRoundStart()
        {
            if (this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum) != null)
            {
                this.speedy = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum).stack / 5;
                if (this.speedy > 5)
                {
                    this.speedy = 5;
                }
                this.owner.ShowPassiveTypo(this);
                this.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, this.speedy, this.owner);
            }
        }
        public override void OnRoundEnd()
        {
            if (nextTarget == null || nextTarget.IsDead())
            {
                return;
            }
            bufAdder.addMark(nextTarget);
        }

        // Token: 0x06000041 RID: 65
        public override void BeforeGiveDamage(BattleDiceBehavior behavior)
        {
            if (this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum) != null)
            {
                this.speedy = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum).stack / 5;
                if (this.speedy > 5)
                {
                    this.speedy = 5;
                }
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmgRate = 10 * this.speedy,
                    breakRate = 10 * this.speedy
                });
            }
        }

        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            owner.battleCardResultLog?.SetPassiveAbility(this);
            bufAdder.addMomentum(this.owner, 1, this.owner);
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            base.OnSucceedAttack(behavior);
            bufAdder.addMomentum(this.owner, 1 , this.owner);
        }

        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {
            if (this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_Momentum).stack > 0)
            {
                base.OnTakeDamageByAttack(atkDice, dmg);
                bufAdder.removeMomentum(this.owner, 1, this.owner);
            }
        }

        public static string Name = "Perry Dancer";

        public static string Desc = "After using a Combat Page on a target, apply {Azure Mark} on them for the next Scene. Only one target can be marked at a time. Gain 1 stack of {Momentum} upon winning a clash and landing hits. Lose 1 stack of {Momentum} upon being hit. For every 5 stacks of {Momentum}, gain 1 Haste every Scene and deal 10% more damage and stagger damage with attacks. This effect can stack up to 5 times. At the start of an Act, gain 10 {Momentum}. ";

        // Token: 0x0400002B RID: 43
        private int speedy;
        private BattleUnitModel nextTarget;
    }
}

