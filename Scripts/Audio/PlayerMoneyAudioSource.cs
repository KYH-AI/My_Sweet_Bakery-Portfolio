using UnityEngine;

public class PlayerMoneyAudioSource : MonoBehaviour
{
    [Header("Audio Components")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip coinClip;
    
    [Header("Musical Scale Settings")] // 도,레,미,파,솔,라,시,도 (8음)
    private float basePitch = 1f;            // 기본 피치 (도)

    [Header("Audio Settings")]
    private bool resetAfterOctave = true;    // 한 옥타브 후 리셋


    // 장조 음계 (도=1.0 기준)
    private readonly float[] majorScale = { 
        1.000f,     // 도 (C)
        1.122f,     // 레 (D)
        1.260f,     // 미 (E)
        1.335f,     // 파 (F)
        1.498f,     // 솔 (G)
        1.682f,     // 라 (A)
        1.888f,     // 시 (B)
        2.000f      // 도 (C) - 한 옥타브 위
    };

    private int currentNoteIndex = 0;

    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.clip = coinClip;
        }
    }
    
    public void PlayCoinSound()
    {
        // 현재 음계의 피치 계산
        float currentPitch = basePitch * majorScale[currentNoteIndex];
        
        // 피치 적용
        audioSource.pitch = currentPitch;
        
        // 소리 재생
        audioSource.PlayOneShot(coinClip);

        // 다음 음으로 이동
        NextNote();
    }
    
    private void NextNote()
    {
        currentNoteIndex++;
        
        // 한 옥타브 완성 시
        if (currentNoteIndex >= majorScale.Length)
        {
            if (resetAfterOctave)
            {
                currentNoteIndex = 0; // 처음으로 돌아가기
            }
            else
            {
                // 계속 올라가기 (더 높은 옥타브)
                currentNoteIndex = 1; // 다음 옥타브의 레부터
                basePitch *= 2f; // 한 옥타브 위로
            }
        }
    }
}
