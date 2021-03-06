using System;
using Antura.AnturaSpace.UI;
using Antura.Audio;
using Antura.Core;
using Antura.Dog;
using Antura.Profile;
using Antura.Tutorial;
using Antura.UI;
using UnityEngine;
using System.Collections;
using System.Linq;
using Antura.Extensions;
using DG.Tweening;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    /// <summary>
    /// Implements a tutorial for the AnturaSpace scene.
    /// </summary>
    public class AnturaSpaceTutorialManager : TutorialManager
    {
        // References
        public AnturaSpaceScene _mScene;

        [SerializeField]
        private Camera m_oCameraUI;
        public AnturaLocomotion m_oAnturaBehaviour;
        public AnturaSpaceUI UI;
        public ShopDecorationsManager ShopDecorationsManager;
        public Button m_oCookieButton;
        public Button m_oPhotoButton;

        [SerializeField]
        private Button m_oCustomizationButton;
        private AnturaSpaceCategoryButton m_oCategoryButton;
        private AnturaSpaceItemButton m_oItemButton;
        private AnturaSpaceSwatchButton m_oSwatchButton;

        protected override AppScene CurrentAppScene
        {
            get { return AppScene.AnturaSpace; }
        }

        protected override void InternalHandleStart()
        {
            // References
            TutorialUI.SetCamera(m_oCameraUI);

            // Play sequential tutorial phases
            switch (FirstContactManager.I.CurrentPhaseInSequence) {
                case FirstContactPhase.AnturaSpace_TouchAntura:
                    StepTutorialTouchAntura();
                    return;

                case FirstContactPhase.AnturaSpace_Customization:
                    StepTutorialCustomization();
                    return;

                case FirstContactPhase.AnturaSpace_Shop:
                    StepTutorialShop();
                    return;

                case FirstContactPhase.AnturaSpace_Exit:
                    StepTutorialExit();
                    return;
            }

            // Play bonus tutorial phases
            bool isPhaseToBeCompleted = IsPhaseToBeCompleted(FirstContactPhase.AnturaSpace_Photo);
            if (isPhaseToBeCompleted)
            {
                StepTutorialPhoto();
                return;
            }

            // If nothing else is to be done, stop the tutorial
            StopTutorialRunning();
        }

        protected override void SetPhaseUIShown(FirstContactPhase phase, bool choice)
        {
            switch (phase)
            {
                case FirstContactPhase.AnturaSpace_Shop:
                    UI.ShowShopButton(choice);
                    if (choice)
                    {
                        ShopDecorationsManager.SetContextClosed();
                    }
                    else
                    {
                        ShopDecorationsManager.SetContextHidden();
                    }
                    break;
                case FirstContactPhase.AnturaSpace_Customization:
                    m_oCustomizationButton.gameObject.SetActive(choice);
                    break;
                case FirstContactPhase.AnturaSpace_Photo:
                    m_oPhotoButton.gameObject.SetActive(choice);
                    break;
                case FirstContactPhase.AnturaSpace_Exit:
                    if (choice)
                    {
                        _mScene.ShowBackButton();
                    }
                    else
                    {
                        _mScene.HideBackButton();
                    }
                    break;
            }
        }

        #region Touch Antura

        private void StepTutorialTouchAntura()
        {
            // Push the player to touch Antura

            TutorialUI.Clear(false);

            // Reset antura as sleeping
            _mScene.Antura.transform.position = _mScene.SceneCenter.position;
            _mScene.Antura.AnimationController.State = AnturaAnimationStates.sleeping;
            _mScene.CurrentState = _mScene.Sleeping;

            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro, null);

            m_oAnturaBehaviour.onTouched += HandleAnturaTouched;

            Vector3 clickOffset = m_oAnturaBehaviour.IsSleeping ? Vector3.down * 2 : Vector3.zero;
            TutorialUI.ClickRepeat(m_oAnturaBehaviour.gameObject.transform.position + clickOffset + Vector3.forward * -2 + Vector3.up,
                float.MaxValue, 1);
        }

        private void HandleAnturaTouched()
        {
            m_oAnturaBehaviour.onTouched -= HandleAnturaTouched;
            CompleteTutorialPhase();
        }

        #endregion

        #region Customization

        private enum CustomizationTutorialStep
        {
            START,
            OPEN_CUSTOMIZE,
            SELECT_CATEGORY,
            SELECT_ITEM,
            SELECT_COLOR,
            CLOSE_CUSTOMIZE,
            FINISH
        }

        private CustomizationTutorialStep _currentCustomizationStep = CustomizationTutorialStep.START;
        private void StepTutorialCustomization()
        {
            if (_currentCustomizationStep < CustomizationTutorialStep.FINISH) _currentCustomizationStep += 1;

            //Debug.Log("CURRENT STEP IS " + _currentCustomizationStep);
            TutorialUI.Clear(false);

            switch (_currentCustomizationStep)
            {
                case CustomizationTutorialStep.OPEN_CUSTOMIZE:
                    AudioManager.I.StopDialogue(false);

                    // Reset state for the tutorial
                    AppManager.I.Player.CurrentAnturaCustomizations.ClearEquippedProps();

                    //dialog get more cookies
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () {
                        //dialog customize
                        AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () {
                            //after the dialog make appear the customization button
                            m_oCustomizationButton.gameObject.SetActive(true);
                            m_oCustomizationButton.onClick.AddListener(StepTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oCustomizationButton.transform.position, float.MaxValue, 1);
                        });
                    });
                    break;


                case CustomizationTutorialStep.SELECT_CATEGORY:

                    m_oCustomizationButton.onClick.RemoveListener(StepTutorialCustomization);
                    _mScene.UI.SetTutorialMode(true);

                    StartCoroutine(DelayedCallbackCO(
                        () => {
                            m_oCategoryButton = _mScene.UI.GetNewCategoryButton();
                            if (m_oCategoryButton == null) throw new Exception("No new category!");
                            m_oCategoryButton.Bt.onClick.AddListener(StepTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oCategoryButton.transform.position, float.MaxValue, 1);
                        }));
                    break;

                case CustomizationTutorialStep.SELECT_ITEM:

                    // Unregister from category button
                    m_oCategoryButton.Bt.onClick.RemoveListener(StepTutorialCustomization);

                    StartCoroutine(DelayedCallbackCO(
                        () => {
                            // Register on item button
                            m_oItemButton = _mScene.UI.GetFirstUnlockedItemButton();
                            if (m_oItemButton == null) throw new Exception("No unlocked item!");
                            m_oItemButton.Bt.onClick.AddListener(StepTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oItemButton.transform.position, float.MaxValue, 1);
                        }));
                    break;

                case CustomizationTutorialStep.SELECT_COLOR:

                    // Cleanup last step
                    _mScene.UI.SetTutorialMode(false);
                    m_oItemButton.Bt.onClick.RemoveListener(StepTutorialCustomization);

                    StartCoroutine(DelayedCallbackCO(
                        () => {
                            // Register on item button
                            m_oSwatchButton = _mScene.UI.GetRandomUnselectedSwatch();
                            m_oSwatchButton.Bt.onClick.AddListener(StepTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oSwatchButton.transform.position, float.MaxValue, 1);
                        }));
                    break;

                case CustomizationTutorialStep.CLOSE_CUSTOMIZE:

                    // Cleanup last step
                    _mScene.UI.SetTutorialMode(false);
                    m_oSwatchButton.Bt.onClick.RemoveListener(StepTutorialCustomization);

                    // New step
                    m_oAnturaBehaviour.onTouched += StepTutorialCustomization;
                    m_oCustomizationButton.onClick.AddListener(StepTutorialCustomization);

                    StartCoroutine(DelayedCallbackCO(
                     () => {
                         TutorialUI.ClickRepeat(m_oCustomizationButton.transform.position, float.MaxValue, 1);
                     }));

                    /*
                    // New step
                    StartCoroutine(WaitAnturaInCenterCO(
                        () => {
                            Vector3 clickOffset = m_oAnturaBehaviour.IsSleeping ? Vector3.down * 2 : Vector3.down * 1.5f;
                            TutorialUI.ClickRepeat(
                                m_oAnturaBehaviour.gameObject.transform.position + clickOffset + Vector3.forward * -2 + Vector3.up,
                                float.MaxValue, 1);
                        }));
                        */
                    break;

                case CustomizationTutorialStep.FINISH:

                    // Cleanup last step
                    m_oCustomizationButton.onClick.RemoveListener(StepTutorialCustomization);
                    m_oAnturaBehaviour.onTouched -= StepTutorialCustomization;

                    CompleteTutorialPhase();
                    break;
            }
        }

        #endregion

        #region Shop

        private enum ShopTutorialStep
        {
            START,
            ENTER_SHOP,
            DRAG_BONE,
            PLACE_NEW_DECORATION,
            CONFIRM_BUY_DECORATION,
            MOVE_DECORATION,
            EXIT_SHOP,
            FINISH
        }

        private ShopTutorialStep _currentShopStep = ShopTutorialStep.START;
        private void StepTutorialShop()
        {
            if (_currentShopStep < ShopTutorialStep.FINISH) _currentShopStep += 1;

            //Debug.Log("WE ARE IN STEP " + _currentShopStep);

            TutorialUI.Clear(false);
            AudioManager.I.StopDialogue(false);

            // Hide other UIs
            SetPhaseUIShown(FirstContactPhase.AnturaSpace_Customization, false);
            SetPhaseUIShown(FirstContactPhase.AnturaSpace_Exit, false);

            ShopActionUI actionUI;
            Button yesButton;

            switch (_currentShopStep)
            {
                case ShopTutorialStep.ENTER_SHOP:

                    AnturaSpaceScene.I.TutorialMode = true;
                    CurrentTutorialFocus = m_oCookieButton;

                    // Start from a clean state
                    AppManager.I.Player.MakeSureInitialBonesAreAvailable();
                    ShopDecorationsManager.I.DeleteAllDecorations();

                    // Dialog -> Appear button
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate
                    {
                        ShopDecorationsManager.SetContextClosed();
                        UI.ShowShopButton(true);

                        m_oCookieButton.onClick.AddListener(StepTutorialShop);
                        TutorialUI.ClickRepeat(m_oCookieButton.transform.position, float.MaxValue, 1);

                        AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_1, null);
                    });
                    break;

                case ShopTutorialStep.DRAG_BONE:

                    // Clean last step
                    m_oCookieButton.onClick.RemoveListener(StepTutorialShop);

                    // New step
                    actionUI = UI.ShopPanelUI.GetActionUIByName("ShopAction_Bone");
                    //actionUI.ShopAction.OnActionCommitted += StepTutorialShop;
                    _mScene.onEatObject += StepTutorialShop;
                    CurrentTutorialFocus = actionUI;

                    // Dialog (drag bone)
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_2);

                    // Start drag line
                    StartCoroutine(DelayedCallbackCO(
                        () =>
                        {
                            StartDrawDragLineFrom(actionUI.transform);
                        }
                    ));

                    // TODO: for stop dragging? needed?
                    //m_bIsDragAnimPlaying = true;
                    // Register delegate to disable draw line after done
                    //UnityEngine.EventSystems.EventTrigger.Entry _oEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                    // _oEntry.eventID = UnityEngine.EventSystems.EventTriggerType.BeginDrag;
                    //_oEntry.callback.AddListener((data) => { m_bIsDragAnimPlaying = false; });
                    //m_oCookieButton.GetComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(_oEntry);

                    break;
                case ShopTutorialStep.PLACE_NEW_DECORATION:

                    // Cleanup last step
                    StopDrawDragLine();
                    //actionUI = UI.ShopPanelUI.GetActionUIByName("ShopAction_Bone");
                    //actionUI.ShopAction.OnActionCommitted -= StepTutorialShop;
                    _mScene.onEatObject -= StepTutorialShop;

                    // New step
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie);
                    ShopDecorationsManager.OnPurchaseConfirmationRequested += StepTutorialShop;

                    actionUI = UI.ShopPanelUI.GetActionUIByName("ShopAction_Decoration_Tree1");
                    var leftmostUnassignedSlot =
                        ShopDecorationsManager.GetDecorationSlots()
                            .Where(x => !x.Assigned && x.slotType == ShopDecorationSlotType.Prop)
                            .MinBy(x => x.transform.position.x);
                    StartDrawDragLineFromTo(actionUI.transform, leftmostUnassignedSlot.transform);

                    CurrentTutorialFocus = actionUI;

                    break;

                case ShopTutorialStep.CONFIRM_BUY_DECORATION:

                    // Cleanup last step
                    StopDrawDragLine();
                    ShopDecorationsManager.OnPurchaseConfirmationRequested -= StepTutorialShop;

                    // New step
                    ShopDecorationsManager.OnPurchaseComplete += StepTutorialShop;

                    yesButton = UI.ShopPanelUI.confirmationYesButton;

                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, () =>
                    {
                        TutorialUI.ClickRepeat(yesButton.transform.position, float.MaxValue, 1);
                    });

                    CurrentTutorialFocus = yesButton;

                    break;

                case ShopTutorialStep.MOVE_DECORATION:

                    // Cleanup last step
                    ShopDecorationsManager.OnPurchaseComplete -= StepTutorialShop;

                    // New step
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie);

                    ShopDecorationsManager.OnDragStop += StepTutorialShop;

                    // Slot we assigned
                    var assignedSlot = ShopDecorationsManager.GetDecorationSlots().FirstOrDefault(x => x.Assigned && x.slotType == ShopDecorationSlotType.Prop);
                    var rightmostUnassignedSlot = ShopDecorationsManager.GetDecorationSlots()
                            .Where(x => !x.Assigned && x.slotType == ShopDecorationSlotType.Prop)
                            .MaxBy(x => x.transform.position.x);
                    StartDrawDragLineFromTo(assignedSlot.transform, rightmostUnassignedSlot.transform);

                    break;

                case ShopTutorialStep.EXIT_SHOP:

                    // Cleanup last step
                    ShopDecorationsManager.OnDragStop -= StepTutorialShop;
                    StopDrawDragLine();

                    yesButton = UI.ShopPanelUI.confirmationYesButton;
                    yesButton.onClick.RemoveListener(StepTutorialShop);

                    // New step
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie);

                    m_oCookieButton.onClick.AddListener(StepTutorialShop);
                    TutorialUI.ClickRepeat(m_oCookieButton.transform.position, float.MaxValue, 1);

                    CurrentTutorialFocus = m_oCookieButton;
                    break;

                case ShopTutorialStep.FINISH:

                    // Cleanup last step
                    m_oCookieButton.onClick.RemoveListener(StepTutorialShop);

                    // New step
                    // dialog: exit and play
                    _mScene.ShowBackButton();
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie);

                    AnturaSpaceScene.I.TutorialMode = false;
                    CurrentTutorialFocus = null;
                    CompleteTutorialPhase();
                    break;
            }
        }

        #endregion

        #region

        private enum PhotoTutorialStep
        {
            START,
            CLICK_PHOTO,
            CONFIRM_PHOTO,
            FINISH
        }

        private PhotoTutorialStep _currentPhotoStep = PhotoTutorialStep.START;
        private void StepTutorialPhoto()
        {
            if (_currentPhotoStep < PhotoTutorialStep.FINISH) _currentPhotoStep += 1;

            TutorialUI.Clear(false);

            // Hide other UIs
            SetPhaseUIShown(FirstContactPhase.AnturaSpace_Customization, false);
            SetPhaseUIShown(FirstContactPhase.AnturaSpace_Exit, false);

            Debug.Log("CURRENT STEP IS " + _currentPhotoStep);
            switch (_currentPhotoStep)
            {
                case PhotoTutorialStep.CLICK_PHOTO:

                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, () =>
                    {
                        m_oPhotoButton.gameObject.SetActive(true);
                        m_oPhotoButton.onClick.AddListener(StepTutorialPhoto);
                        TutorialUI.ClickRepeat(m_oPhotoButton.transform.position, float.MaxValue, 1);
                    });
                    break;

                case PhotoTutorialStep.CONFIRM_PHOTO:

                    // Cleanup last step
                    m_oPhotoButton.onClick.RemoveListener(StepTutorialPhoto);

                    // New step
                    ShopPhotoManager.I.OnPurchaseCompleted += StepTutorialPhoto;
                    var yesButton = UI.ShopPanelUI.confirmationYesButton;

                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie);

                    StartCoroutine(DelayedCallbackCO(() =>
                    {
                        TutorialUI.ClickRepeat(yesButton.transform.position, float.MaxValue, 1);
                    }));

                    break;
                case PhotoTutorialStep.FINISH:

                    // Cleanup last step
                    ShopPhotoManager.I.OnPurchaseCompleted -= StepTutorialPhoto;

                    // New step
                    // dialog: exit and play
                    _mScene.ShowBackButton();
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie);

                    CompleteTutorialPhase();
                    break;
            }
        }
        #endregion

        #region Exit

        private void StepTutorialExit()
        {
            TutorialUI.Clear(false);

            _mScene.ShowBackButton();

            AudioManager.I.StopDialogue(false);

            TutorialUI.ClickRepeat(
                Vector3.down * 0.025f + m_oCameraUI.ScreenToWorldPoint(new Vector3(GlobalUI.I.BackButton.RectT.position.x,
                    GlobalUI.I.BackButton.RectT.position.y, m_oCameraUI.nearClipPlane)), float.MaxValue, 1);
        }

        #endregion

        #region Utility functions

        IEnumerator DelayedCallbackCO(System.Action callback)
        {
            yield return new WaitForSeconds(0.6f);

            if (callback != null) {
                callback();
            }
        }

        /*
        IEnumerator WaitAnturaInCenterCO(System.Action callback)
        {
            while (!_mScene.Antura.IsNearTargetPosition || _mScene.Antura.IsSliping)
                yield return null;

            if (callback != null) {
                callback();
            }
        }*/

        private TutorialUIAnimation dragLineAnimation;

        private void StartDrawDragLineFromTo(Transform fromTr, Transform toTr)
        {
            TutorialUI.Clear(false);

            Vector3[] path = new Vector3[3];
            path[0] = fromTr.position;
            path[2] = toTr.position;
            path[1] = (path[0] + path[2])/2f + Vector3.up * 4 + Vector3.left * 2;

            dragLineAnimation = TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.Finger, false, true);
            dragLineAnimation.MainTween.timeScale = 0.8f;
            dragLineAnimation.OnComplete(delegate {
                StartDrawDragLineFromTo(fromTr, toTr);
            });
        }

        private void StartDrawDragLineFrom(Transform fromTr)
        {
            TutorialUI.Clear(false);

            Vector3[] path = new Vector3[3];
            path[0] = fromTr.position;
            path[1] = path[0] + Vector3.up * 4 + Vector3.left * 2;
            path[2] = m_oCameraUI.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));
            path[2].z = path[1].z;

            dragLineAnimation = TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.Finger);
            dragLineAnimation.MainTween.timeScale = 0.8f;
            dragLineAnimation.OnComplete(delegate  {
                StartDrawDragLineFrom(fromTr) ;
            });
        }

        private void StopDrawDragLine()
        {
            if (dragLineAnimation != null)
            {
                dragLineAnimation.MainTween.Kill();
                dragLineAnimation = null;
            }
        }

        #endregion
    }
}