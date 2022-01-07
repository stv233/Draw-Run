using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))] 
    public class Drawable : MonoBehaviour
    {
        public static Color PenColour = Color.red;
        public static int PenWidth = 8;
        public static event Action<List<Vector2>, Rect> OnDrawEnd;

        public delegate void BrushFunction(Vector2 world_position);

        public BrushFunction currentBrush;
        public LayerMask DrawingLayers;
        public bool ResetCanvasOnPlay = true;
        public Color ResetColour = new Color(0, 0, 0, 0);
        public static Drawable drawable;

        [SerializeField] private Camera drawingCamera;

        private Sprite drawableSprite;
        private Texture2D drawableTexture;
        private Texture2D defaultTexture;
        private Vector2 previousDragPosition;
        private Color[] cleanColoursArray;
        private Color transparent;
        private Color32[] currentColors;
        private bool mouseWasPreviouslyHeldDown = false;
        private bool noDrawingOnCurrentDrag = false;
        private List<Vector2> points = new List<Vector2>();




//////////////////////////////////////////////////////////////////////////////
// BRUSH TYPES. Implement your own here


        // When you want to make your own type of brush effects,
        // Copy, paste and rename this function.
        // Go through each step
        public void BrushTemplate(Vector2 world_position)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixelPos = WorldToPixelCoordinates(world_position);

            // 2. Make sure our variable for pixel array is updated in this frame
            currentColors = drawableTexture.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)

            // If you do care about dragging, use the below if/else structure
            if (previousDragPosition == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HERE
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixelPos, PenWidth, PenColour);
            }
            else
            {
                // THE USER IS DRAGGING
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previousDragPosition, pixelPos, PenWidth, PenColour);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();
            
            // 4. If dragging, update where we were previously
            previousDragPosition = pixelPos;
        }



        
        // Default brush type. Has width and colour.
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector2 worldPoint)
        {
            Vector2 pixelPos = WorldToPixelCoordinates(worldPoint);

            currentColors = drawableTexture.GetPixels32();

            if (previousDragPosition == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixelPos, PenWidth, PenColour);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previousDragPosition, pixelPos, PenWidth, PenColour);
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previousDragPosition = pixelPos;
        }


        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            currentBrush = PenBrush;
        }
//////////////////////////////////////////////////////////////////////////////


        // This is where the magic happens.
        // Detects when user is left clicking, which then call the appropriate function
        void Update()
        {
            // Is the user holding down the left mouse button?
            bool mouseHeldDown = Input.GetMouseButton(0);
            if (mouseHeldDown && !noDrawingOnCurrentDrag)
            {
                // Convert mouse coordinates to world coordinates
                Vector2 mouseWorldPosition = drawingCamera.ScreenToWorldPoint(Input.mousePosition);

                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, DrawingLayers.value);
                if (hit != null && hit.transform != null)
                {
                    // We're over the texture we're drawing on!
                    // Use whatever function the current brush is
                    currentBrush(mouseWorldPosition);
                }

                else
                {
                    // We're not over our destination texture
                    previousDragPosition = Vector2.zero;
                    if (!mouseWasPreviouslyHeldDown)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        noDrawingOnCurrentDrag = true;
                    }
                }
            }
            // Mouse is released
            else if (!mouseHeldDown)
            {
                if (points.Count > 0)
                {
                    Debug.Log("Send");
                    OnDrawEnd?.Invoke(points.Distinct().ToList(), new Rect(0, 0, drawableTexture.width, drawableTexture.height));
                }
                ResetToDefault();
                Init();
                previousDragPosition = Vector2.zero;
                noDrawingOnCurrentDrag = false;
            }
            mouseWasPreviouslyHeldDown = mouseHeldDown;
        }



        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 startPoint, Vector2 endPoint, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(startPoint, endPoint);
            Vector2 direction = (startPoint - endPoint).normalized;

            Vector2 cur_position = startPoint;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(startPoint, endPoint, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }


        public void MarkPixelsToColour(Vector2 centerPixel, int penThickness, Color colorOfPen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int centerX = (int)centerPixel.x;
            int centerY = (int)centerPixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawableSprite.rect.width || x < 0)
                    continue;

                for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
                {
                    MarkPixelToChange(x, y, colorOfPen);
                    points.Add(new Vector2(x, y));
                }
            }
        }
        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int arrayPos = y * (int)drawableSprite.rect.width + x;

            // Check if this is a valid position
            if (arrayPos > currentColors.Length || arrayPos < 0)
                return;

            currentColors[arrayPos] = color;
        }
        public void ApplyMarkedPixelChanges()
        {
            drawableTexture.SetPixels32(currentColors);
            drawableTexture.Apply();
        }


        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 centerPixel, int penThickness, Color colorOfPen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int centerX = (int)centerPixel.x;
            int centerY = (int)centerPixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
            {
                for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
                {
                    drawableTexture.SetPixel(x, y, colorOfPen);
                    points.Add(new Vector2(x, y));
                    Debug.Log($"{points.Count}");
                }
            }

            drawableTexture.Apply();
        }


        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // Change coordinates to local coordinates of this image
            Vector3 localPos = transform.InverseTransformPoint(world_position);

            // Change these to coordinates of pixels
            float pixelWidth = drawableSprite.rect.width;
            float pixelHeight = drawableSprite.rect.height;
            float unitsToPixels = pixelWidth / drawableSprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centeredX = localPos.x * unitsToPixels + pixelWidth / 2;
            float centeredY = localPos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centeredX), Mathf.RoundToInt(centeredY));

            return pixel_pos;
        }


        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawableTexture.SetPixels(cleanColoursArray);
            drawableTexture.Apply();
        }

     
        void Awake()
        {
            drawable = this;
            // DEFAULT BRUSH SET HERE
            currentBrush = PenBrush;

            drawableSprite = this.GetComponent<SpriteRenderer>().sprite;
            defaultTexture = drawableSprite.texture;
            ResetToDefault();

            Init();
             
        }

        public void Init()
        {
            // Initialize clean pixels to use
            cleanColoursArray = new Color[(int)drawableSprite.rect.width * (int)drawableSprite.rect.height];
            for (int x = 0; x < cleanColoursArray.Length; x++)
                cleanColoursArray[x] = ResetColour;

            // Should we reset our canvas image when we hit play in the editor?
            if (ResetCanvasOnPlay)
                ResetCanvas();
        }

        public void ResetToDefault()
        {
            points.Clear();
            drawableTexture = Instantiate(defaultTexture) as Texture2D;
            drawableSprite = Sprite.Create(drawableTexture, new Rect(0, 0, drawableTexture.width, drawableTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            this.GetComponent<SpriteRenderer>().sprite = drawableSprite;
        }
    }
}