using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FxRumbleTransform : MonoBehaviour
{
    //#region design vars
    //public float m_OneWayLength = 0.1f;
    //[Range(0.0f, 1.0f)]
    //public float m_InclineAgressivness = 0.5f;
    //[Range(0.0f, 1.0f)]
    //public float m_DeclineAgressivness = 0.5f;
    //#endregion

    //private Transform m_tFormOwner;
    //private Vector3 m_TargetRumblePos;
    //private Vector3 m_RumbleDir;
    //private float m_TargetLength;
    //private float m_NowRanLength;
    //private float m_DeclineStepLength;

    //private enum EState
    //{
    //    ENTER,
    //    INCLINE,
    //    DECLINE,
    //    EXIT
    //}
    //private EState m_NowState;




    private Transform m_tForm;
    private Vector3 m_InitLocalPos;
    private float m_Intensity;
    private float m_Decrease;
    private float m_MultiUp;
    private float m_MultiUpNow;
    private float m_MultiDown;
    private bool m_DirFlipper;
    private bool m_IsRumble;


    // ----------------------------------------------------------------------------------------------------


    public void Init(Transform tForm, float intensity, float decrease, float multiUp, float multiDown)
    {
        m_tForm = tForm;
        m_InitLocalPos = new Vector3(m_tForm.transform.localPosition.x, m_tForm.transform.localPosition.y, m_tForm.transform.localPosition.z);
        m_Intensity = intensity;
        m_Decrease = decrease;
        m_MultiUp = multiUp;
        m_MultiUpNow = m_MultiUp;
        m_MultiDown = multiDown;
        m_DirFlipper = false;
        m_IsRumble = false;
    }


    public void Rumble()    
    {
        if (m_IsRumble != true)
        {
            m_InitLocalPos = new Vector3(m_tForm.transform.localPosition.x, m_tForm.transform.localPosition.y, m_tForm.transform.localPosition.z);
            m_IsRumble = true;
        }
    }


    private void ResetToOriginValues()
    {
        m_MultiUpNow = m_MultiUp;
        m_IsRumble = false;
    }


    private void Rumbler()
    {
        if (m_IsRumble == true)
        {
            if (m_DirFlipper == false)
            {
                Vector3 vec = new Vector3(m_MultiUpNow, m_MultiUpNow, 0.0f) * Time.deltaTime;
                m_tForm.localPosition += vec;
                if (m_tForm.localPosition.x > m_InitLocalPos.x + m_Intensity)
                {
                    m_tForm.localPosition = new Vector3(m_Intensity + m_InitLocalPos.x, m_Intensity + m_InitLocalPos.y, m_Intensity + m_InitLocalPos.z);
                    m_MultiUpNow -= m_Decrease;
                    m_DirFlipper = true;
                }
            }
            else
            {
                m_tForm.localPosition -= new Vector3(m_MultiDown, m_MultiDown, 0.0f) * Time.deltaTime;
                if (m_tForm.localPosition.x < m_InitLocalPos.x)
                {
                    m_tForm.localPosition = m_InitLocalPos;
                    m_DirFlipper = false;

                    if (m_MultiUp < 0.1f)
                    {
                        ResetToOriginValues();
                    }
                }
            }
        }
    }


    private void Awake()
    {
        m_tForm = null;
        m_InitLocalPos = Vector3.zero;
        m_Intensity = 0.0f;
        m_Decrease = 0.0f;
        m_MultiUp = 0.0f;
        m_MultiDown = 0.0f;
        m_DirFlipper = false;
        m_IsRumble = false;
    }


    private void Update()
    {
        Rumbler();
    }




    //public void Init(Transform tFormToModulate)
    //{
    //    m_tFormOwner = tFormToModulate;
    //    m_TargetRumblePos = Vector3.zero;
    //    m_RumbleDir = Vector3.zero;
    //    m_TargetLength = m_OneWayLength;
    //    m_NowRanLength = 0.0f;
    //    m_DeclineStepLength = m_OneWayLength / m_DeclineAgressivness;
    //    m_IsRumble = false;

    //    m_NowState = EState.ENTER;
    //}


    //public void ExecuteRumble()     // call this from your controlling script once to start a one rumble sequence
    //{
    //    m_IsRumble = true;
    //}


    //private void RunRumbleStates()
    //{
    //    switch(m_NowState)
    //    {
    //        case EState.ENTER:      EnterState();   break;
    //        case EState.INCLINE:    InclineState(); break;
    //        case EState.DECLINE:    DeclineState(); break;
    //        case EState.EXIT:       ExitState();    break;
    //        default:                                break;
    //    }
    //}


    //private void EnterState()
    //{
    //    m_RumbleDir = new Vector3(
    //        Random.Range(-1.0f, 1.0f),
    //        Random.Range(-1.0f, 1.0f),
    //        Random.Range(-1.0f, 1.0f));

    //    m_NowRanLength = Random.Range(0.0f, m_TargetLength);
    //    Vector3 vec = m_RumbleDir * m_NowRanLength;
    //    m_TargetRumblePos = m_tFormOwner.position + vec;
    //    m_NowState = EState.INCLINE;
    //}


    //private void InclineState()
    //{
    //    m_tFormOwner.position = Vector3.Lerp(m_tFormOwner.position,
    //        m_TargetRumblePos,
    //        m_InclineAgressivness);

    //    if (InTargetRange(m_tFormOwner.position, m_TargetRumblePos, 0.01f) == true)
    //    {
    //        m_tFormOwner.position = m_TargetRumblePos;
    //        m_NowState = EState.DECLINE;
    //    }
    //}


    //private void DeclineState()
    //{
    //    m_tFormOwner.position = Vector3.Lerp(Vector3.zero,
    //        m_tFormOwner.position,
    //        m_DeclineAgressivness);

    //    if(InTargetRange(m_tFormOwner.position, Vector3.zero, 0.01f) == true)
    //    {
    //        m_TargetLength -= m_DeclineStepLength;
    //        m_NowState = EState.EXIT;
    //    }
    //}


    //private void ExitState()
    //{
    //    if (m_TargetLength <= 0.0f)
    //    {
    //        m_IsRumble = false;
    //    }

    //    m_NowState = EState.ENTER;
    //}


    //private bool InTargetRange(Vector3 nowPos, Vector3 targetPos, float range = 0.01f)
    //{
    //    if(nowPos == targetPos)
    //    {
    //        return true;
    //    }

    //    //if ((nowPos.x < targetPos.x + range && nowPos.x > targetPos.x - range) &&
    //    //   (nowPos.y < targetPos.y + range && nowPos.y > targetPos.y - range) &&
    //    //   (nowPos.x < targetPos.z + range && nowPos.z > targetPos.z - range))
    //    //{
    //    //    return true;
    //    //}

    //    return false;
    //}


    //private void Update()
    //{
    //    if(m_IsRumble == true)
    //    {
    //        RunRumbleStates();
    //    }
    //}
}
