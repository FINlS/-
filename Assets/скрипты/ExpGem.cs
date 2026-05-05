using UnityEngine;

public class ExpGem : MonoBehaviour
{
    public int expAmount = 10;     // Сколько опыта дает
    public float collectDistance = 1.5f; // Дистанция подбора
    public float magnetSpeed = 10f;      // Скорость полета к игроку
    
    private Transform player;
    private bool isFollowing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Если игрок подошел достаточно близко, включаем магнит
        if (distance <= collectDistance)
        {
            isFollowing = true;
        }

        if (isFollowing)
        {
            // Летим к игроку
            transform.position = Vector3.MoveTowards(transform.position, player.position, magnetSpeed * Time.deltaTime);

            // Если долетели совсем близко — поглощаемся
            if (distance < 0.2f)
            {
                player.GetComponent<PlayerStats>().AddExperience(expAmount);
                Destroy(gameObject);
            }
        }
    }
}