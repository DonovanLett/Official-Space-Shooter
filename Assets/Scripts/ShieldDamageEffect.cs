using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDamageEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // To control the visual appearance (color/alpha) of the shield sprite

    [Header("Flicker Settings")] // Creates a labeled section in Unity’s Inspector
    [SerializeField] private float flickerDuration = 0.2f; // Total time the shield flickers when hit
    [SerializeField] private int flickerCount = 2;         // How many flickers occur per hit
    [SerializeField] private float flickerAlphaMin = 0.3f; // Minimum transparency level during a flicker

    [Header("Hit Settings")] // Another labeled section in the Inspector
    [SerializeField] private int maxHits = 3;        // Max number of hits the shield can take
    [SerializeField] private float minBaseAlpha = 0.2f; // Shield's transparency after taking max hits

    private Color originalColor; // Stores the original color of the shield so we can fade from it
    private int currentHits = 0; // Tracks how many hits the shield has taken

    private void Awake() // Called when the object is first initialized
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer on the GameObject
        originalColor = spriteRenderer.color;            // Save the original color (includes alpha)
    }

    public void FlickerOnHit() // Call this when the shield is hit
    {
        currentHits = Mathf.Min(currentHits + 1, maxHits); // Increase hit count but don't exceed maxHits
        UpdateBaseAlpha();                                 // Fade the shield based on damage taken
        StopAllCoroutines();                               // Stop any flicker already running
        StartCoroutine(FlickerRoutine());     // Start a new flicker effect
        if(currentHits == maxHits - 1)
        {
            StartCoroutine(FadingAnimation());
        }
    }

    private void UpdateBaseAlpha() // Fades the shield based on how many hits it has taken
    {
        float hitProgress = (float)currentHits / maxHits; // Value between 0 (no hits) and 1 (max hits)
        float newAlpha = Mathf.Lerp(originalColor.a, minBaseAlpha, hitProgress);
        // Linearly interpolate alpha from original to dimmest based on hitProgress

        Color updatedColor = spriteRenderer.color; // Get current color
        updatedColor.a = newAlpha;                 // Apply new alpha
        spriteRenderer.color = updatedColor;       // Update the shield's color


    }



    private IEnumerator FlickerRoutine() // Flickering animation using a coroutine
    {
        float baseAlpha = spriteRenderer.color.a; // Current (post-fade) alpha
        float flickerToAlpha = Mathf.Max(flickerAlphaMin, baseAlpha - 0.2f);
        // Make sure we don't flicker below the minimum allowed alpha

        float singleFlickerTime = flickerDuration / (flickerCount * 2f);
        // Each flicker has a fade out and fade in, so total time is divided accordingly

        for (int i = 0; i < flickerCount; i++) // Loop for the number of flickers
        {
            SetAlpha(flickerToAlpha);             // Flicker: temporarily make the shield dimmer
            yield return new WaitForSeconds(singleFlickerTime); // Wait a short time

            SetAlpha(baseAlpha);                  // Return to the normal faded alpha
            yield return new WaitForSeconds(singleFlickerTime); // Wait again
        }
    }

    private void SetAlpha(float alpha) // Utility to set just the alpha of the shield's color
    {
        Color c = spriteRenderer.color; // Get current color
        c.a = alpha;                    // Change the alpha value
        spriteRenderer.color = c;      // Apply updated color to the SpriteRenderer
    }



    public void ResetShield()
    {
        currentHits = 0;
        spriteRenderer.color = originalColor;
    }

    IEnumerator FadingAnimation()
    {
        
        float originalColorAlpha = spriteRenderer.color.a; // The initial Brightness


        float colorAlpha = originalColorAlpha; // The current Brightness as it is manipulated in this Coroutine


        bool goingDown = true; // Whether or not the Brightness is currently going up or down.


        float minColor = 20f;  // How low the Brightness can drop before rising back up to originalColorAlpha


        while (currentHits == maxHits - 1) //This while loop will play so long as we are on one hit left
        {
            if (goingDown)
            {
                colorAlpha -= 0.1f; // When goingDown equals true, the brightness will drop by 0.1

                SetAlpha(colorAlpha); // Every time the colorAlpha is changed, we set it to also change the actual Alpha Color

                if (colorAlpha <= minColor) // If color Alpha is less than or equal to minColor, 
                {
                    goingDown = false; // Set goingDown to false
                }
            }
            else
            {
                colorAlpha += 0.1f; // When goingDown equals false, the brightness will go up by 0.1

                SetAlpha(colorAlpha);

                if (colorAlpha >= originalColorAlpha) // If color Alpha is more than or equal to originalColorAlpha
                {
                    goingDown = true; // Set goingDown to true
                }
            }
            
            yield return new WaitForSeconds(0.2f); // 0.2 second pause between every change in brightness, otherwise the change will be unseeable due to speed.
        }
    }
}