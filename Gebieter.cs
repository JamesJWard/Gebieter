using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Gebieter
{
    //James' World

    public class Map
    {
        public Rectangle spriteRectangle;
        public Texture2D spriteTexture;

        protected float DisplayWidth;

        public Map(
            Texture2D intexture,
            float inX,
            float inY,
            float inDisplayW)
        {
            spriteTexture = intexture;
            DisplayWidth = inDisplayW;

            spriteRectangle.X = (int)inX;
            spriteRectangle.Y = (int)inY;
            spriteRectangle.Width = (int)DisplayWidth * 3;
            spriteRectangle.Height = spriteRectangle.Width;
        }
        /// <summary>
        /// Loads the texture to be used in the sprite.
        /// </summary>
        /// <param name="spriteTexture">The texture to be used.</param>
        public void LoadTexture(Texture2D inSpriteTexture)
        {
            spriteTexture = inSpriteTexture;
        }

        /// <summary>
        /// moves units/screen to the scrolling
        /// </summary>
        /// <param name="direction">true = up/down false = left/right </param>
        /// <param name="degree">amount to be added/minused</param>
        public virtual void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") spriteRectangle.Y -= 4;
                if (direction == "down") spriteRectangle.Y += 4;
                //moves left/right
                if (direction == "left") spriteRectangle.X -= 4;
                if (direction == "right") spriteRectangle.X += 4;
            }
            else
            {
                spriteRectangle.Y = (int)(jumpy + .5f);
                spriteRectangle.X = (int)(jumpx + .5f);
            }
        }

        /// <summary>
        /// draws the sprite
        /// </summary>
        /// <param name="spritebatch">what it's using to draw with</param>
        public virtual void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(spriteTexture, spriteRectangle, Color.LightGray);
        }
    }

    public class baseSprite
    {
        public string indicator;

        #region rectangle/texture/movement
        protected Rectangle sourceRec;
        public Rectangle spriteRectangle;
        protected Texture2D spriteTexture;

        protected float DisplayWidth;
        protected float DisplayHeight;

        protected float initialX;
        protected float initialY;

        public float X;
        public float Y;

        protected int direction;
        protected int directionswitchcounter;

        protected int movedegree;
        public bool inTransport;
        #endregion

        #region animation
        protected int frameWidth;
        protected int frameHeight;
        protected int updateTickCounter = 0;
        protected int rowNumber;
        //
        public bool connected;
        #endregion

        #region combat

        public bool incombat;
        public float health;

        #endregion

        #region explosion
        protected Texture2D explodeTexture;
        protected Rectangle explodeSourceRect;
        private int explodeFrameWidth;
        private int explodeTickCounter;
        protected bool exploding;
        protected SoundEffect explodeSound;
        public bool dead;
        #endregion

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public baseSprite(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            Texture2D explosionTex,
            SoundEffect explosionSound)
        {
            health = inhealth;
            initialX = inInitialX;
            initialY = inInitialY;
            DisplayWidth = inDisplayWidth;
            DisplayHeight = inDisplayHieght;
            frameWidth = inFrameWidth;
            frameHeight = inFrameHeight;
            LoadTexture(inSpriteTexture);

            sourceRec = new Rectangle(0, 0, frameWidth, frameHeight);

            spriteRectangle.Width = (int)((DisplayWidth * widthFactor) + 0.5f);
            float sourceRatio = (float)frameWidth / frameHeight;
            spriteRectangle.Height = (int)((spriteRectangle.Width / sourceRatio) + 0.5f);

            explodeTexture = explosionTex;
            explodeSound = explosionSound;
            explodeFrameWidth = 60;
            explodeSourceRect = new Rectangle(0, 0, 60, 60);
        }

        /// <summary>
        /// Loads the texture to be used in the sprite.
        /// </summary>
        /// <param name="spriteTexture">The texture to be used.</param>
        public void LoadTexture(Texture2D inSpriteTexture)
        {
            spriteTexture = inSpriteTexture;
        }

        /// <summary>
        /// moves units/screen to the scrolling
        /// </summary>
        /// <param name="direction">true = up/down false = left/right </param>
        /// <param name="degree">amount to be added/minused</param>
        public virtual void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") Y -= movedegree;
                if (direction == "down") Y += movedegree;
                //moves left/right
                if (direction == "left") X -= movedegree;
                if (direction == "right") X += movedegree;
            }
            else
            {
                X = jumpx;
                Y = jumpy;
            }
        }

        /// <summary>
        /// updates sprite
        /// </summary>
        /// <param name="game">game in which is being updated in</param>
        public virtual void Update(Gebieter game)
        {
            spriteRectangle.X = (int)(X + 0.5f);
            spriteRectangle.Y = (int)(Y + 0.5f);

            //keeps units from pushing each other out
            if (spriteRectangle.X < (game.mAp1.spriteRectangle.X + (DisplayWidth / 10))) 
                spriteRectangle.X = (int)(game.mAp1.spriteRectangle.X + (DisplayWidth / 10));

            if (spriteRectangle.Right > game.mAp1.spriteRectangle.Right - DisplayWidth/10) 
                spriteRectangle.X = (int)(game.mAp1.spriteRectangle.Right - (DisplayWidth/10) - spriteRectangle.Width);

            if (spriteRectangle.Y < game.mAp1.spriteRectangle.Y + DisplayWidth/10) 
                spriteRectangle.Y = (int)(game.mAp1.spriteRectangle.Y + DisplayWidth/10);

            if (spriteRectangle.Bottom > game.mAp1.spriteRectangle.Bottom-DisplayWidth/10) 
                spriteRectangle.Y = (int)(game.mAp1.spriteRectangle.Bottom - (DisplayWidth/10) - spriteRectangle.Height);

            //life management

            if (indicator != "flyer" && indicator != "tran")
            {
                if (health <= 0) Explode();
            }
            else
            {
                if (health <= 0)
                {
                    if (indicator == "tran")
                    {
                        if (spriteRectangle.Width >= DisplayWidth * .04f)
                        {
                            spriteRectangle.X += 4;
                            spriteRectangle.Width--;
                            spriteRectangle.Y += 4;
                            spriteRectangle.Height--;
                        }
                        else Explode();
                    }
                    else
                    {
                        if (spriteRectangle.Width >= DisplayWidth * .03f)
                        {
                            spriteRectangle.X += 4;
                            spriteRectangle.Width--;
                            spriteRectangle.Y += 4;
                            spriteRectangle.Height--;
                        }
                        else Explode();
                    }
                }
            }
            if (connected)
            {
                #region animation
                updateTickCounter++;

                if (updateTickCounter == 15)
                {
                    updateTickCounter = 0;

                    if (direction == 0)
                    {
                        if (sourceRec.X + frameWidth >= spriteTexture.Width)
                        {
                            // reset the animation to the start frame
                            sourceRec.X = 0;
                        }
                        else
                        {
                            //Move on to the next frame
                            sourceRec.X += frameWidth;
                        }
                    }
                    else
                    {
                        if (directionswitchcounter == 4)
                        {
                            directionswitchcounter = 0;
                            direction *= -1;
                        }
                        directionswitchcounter++;

                        sourceRec.X += (frameWidth * direction);
                    }
                }
                #endregion
            }

            #region exploding
            if (exploding)
            {
                explodeTickCounter++;
                if (explodeTickCounter == 8)
                {
                    explodeTickCounter = 0;
                    if (explodeSourceRect.X + explodeFrameWidth >= explodeTexture.Width)
                    {
                        // reached the end of the sequence
                        // not exploding any more
                        dead = true;
                        exploding = false;
                    }
                    else
                    {
                        // Move on to the next frame
                        explodeSourceRect.X += explodeFrameWidth;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// draws the sprite
        /// </summary>
        /// <param name="spritebatch">what it's using to draw with</param>
        public virtual void Draw(SpriteBatch spritebatch)
        {
            if (exploding || dead)
            {
                spritebatch.Draw(explodeTexture, spriteRectangle,
                                 explodeSourceRect, Color.White);
            }
            else spritebatch.Draw(spriteTexture, spriteRectangle, sourceRec, Color.White);
        }

        /// <summary>
        /// updates the health of a sprite
        /// </summary>
        /// <param name="change">amount to add or subtract from a sprite</param>
        public void UpdateHealth(float change)
        {
            health += change;
        }

        /// <summary>
        /// runs the death animation of units
        /// </summary>
        public void Explode()
        {
            if (exploding)
            {
                return;
            }
            explodeSound.Play();
            explodeSourceRect.X = 0;
            explodeTickCounter = 0;
            exploding = true;
        }

        public bool IsExploding
        {
            get
            {
                return exploding;
            }
        }

        /// <summary>
        /// sets row on sprite
        /// </summary>
        /// <param name="newRowNuumber">which row to move to.</param>
        /// <returns>returns if the row is on the texture or not</returns>
        public bool SetRow(int newRowNuumber)
        {
            int rowY = newRowNuumber * frameHeight;

            if (rowY + frameHeight > spriteTexture.Height)
            {
                // This row does not exist
                return false;
            }

            sourceRec.Y = rowY;
            rowNumber = newRowNuumber;
            return true;
        }
    }

    #region Buildings

    //human
    public class HumanBuilding : baseSprite
    {
        protected KeyboardState oldkeys;
        protected int buildprogression;
        protected int buildFinishTime;

        public Texture2D UnitButton1Tex;
        public Texture2D UnitButton2Tex;
        public Rectangle UnitButton1Rec;
        public Rectangle UnitButton2Rec;

        public int unit1BuildTime;
        public int unit2BuildTime;
        public int Units1ToBuild;
        public int Units2ToBuild;
        public int unit1buildcost;
        public int unit2buildcost;

        protected Rectangle unitProducedWaypoint;
        int disY;
        int disX;

        public HumanBuilding(
            Texture2D inSpriteTexture,
            Texture2D inUnit1,
            Texture2D inUnit2,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, explosionTex, explosionSound)
        {
            connected = true;
            spriteTexture = inSpriteTexture;
            initialX = inX;
            initialY = inY;
            buildFinishTime = inbuildtime;
            UnitButton1Tex = inUnit1;
            UnitButton2Tex = inUnit2;
            X = (int)(initialX);
            Y = (int)(initialY);
            unitProducedWaypoint.X = (int)X;
            unitProducedWaypoint.Y = (int)Y;
            movedegree = 4;
        }

        #region state setup

        public enum BaseBuildingState
        {
            Building,
            Built,
            selected,
        }

        public BaseBuildingState state;

        #endregion

        public override void Update(Gebieter game)
        {
            KeyboardState keys = Keyboard.GetState();
            UnitButton1Rec = new Rectangle((int)(DisplayWidth / 1.91f), (int)(DisplayHeight / 1.097f), (int)(DisplayWidth / 18), (int)(DisplayHeight / 13));
            UnitButton2Rec = new Rectangle((int)(DisplayWidth / 1.73f), (int)(DisplayHeight / 1.097f), (int)(DisplayWidth / 18), (int)(DisplayHeight / 13));

            //if building is in progress of being built
            if (buildprogression < buildFinishTime) state = BaseBuildingState.Building;

            #region state switch
            switch (state)
            {
                case BaseBuildingState.Building:
                    buildprogression++;

                    if (buildprogression <= (buildFinishTime / 3)) { SetRow(4); }
                    if (buildprogression > (buildFinishTime / 3) && buildprogression <= ((buildFinishTime / 3) * 2)) { SetRow(2); }
                    if (buildprogression > (buildFinishTime / 3) * 2) { SetRow(3); }

                    //building has been built
                    if (buildprogression == buildFinishTime) { state = BaseBuildingState.Built; }
                    break;
                case BaseBuildingState.Built:
                    SetRow(0);
                    if (game.clicked == true && game.CursorRec.Intersects(spriteRectangle) && game.CursorRec.Intersects(game.HudRec) == false)
                    {
                        state = BaseBuildingState.selected;
                    }
                    break;
                case BaseBuildingState.selected:
                    SetRow(1);
                    if (game.cancel || game.selecting || game.CursorRec.Intersects(spriteRectangle) == false && game.CursorRec.Intersects(UnitButton1Rec) == false && game.CursorRec.Intersects(UnitButton2Rec) == false
                        && game.CursorRec.Intersects(game.miniMap) == false &&  game.clicked == true)
                        state = BaseBuildingState.Built;

                    if (game.setWaypoint == true)
                    {
                        disX = game.CursorRec.X - spriteRectangle.X;
                        disY = game.CursorRec.Y - spriteRectangle.Y;
                    }
                    unitProducedWaypoint.X = spriteRectangle.X + disX;
                    unitProducedWaypoint.Y = spriteRectangle.Y + disY;
                    break;
            }
            #endregion

            oldkeys = keys;

            base.Update(game);
        }
    }

    public class HumanAirFactory : HumanBuilding
    {
        public HumanAirFactory(
            Texture2D inSpriteTexture,
            Texture2D inBuildFLyer,
            Texture2D inBuildTansport,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound)
            : base(inSpriteTexture, inBuildFLyer, inBuildTansport, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime, explosionTex, explosionSound)
        {
            indicator = "airfact";
            spriteTexture = inSpriteTexture;
            initialX = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
            unit1BuildTime = 420;
            unit2BuildTime = 480;
            unit1buildcost = 50;
            unit2buildcost = 100;
        }

        public override void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") { unitProducedWaypoint.Y -= movedegree; }
                if (direction == "down") { unitProducedWaypoint.Y += movedegree; }
                //moves left/right
                if (direction == "left") { unitProducedWaypoint.X -= movedegree; }
                if (direction == "right") { unitProducedWaypoint.X += movedegree; }
            }
                base.move(direction, jump, jumpx, jumpy);
        }

        public override void Update(Gebieter game)
        {
            if (state == BaseBuildingState.selected)
            {
                if (game.clicked)
                {
                    if (game.CursorRec.Intersects(UnitButton1Rec) && game.CursorRec.X < UnitButton1Rec.Right)
                    {
                        if (game.resouces > unit1buildcost)
                        {
                            Units1ToBuild++;
                            game.resouces -= unit1buildcost;
                        }
                        else game.resourceError.Play();
                    }
                    if (game.CursorRec.Intersects(UnitButton2Rec) && game.CursorRec.X > UnitButton2Rec.X)
                    {
                        if (game.resouces > unit2buildcost)
                        {
                            Units2ToBuild++;
                            game.resouces -= unit2buildcost;
                        }
                        else game.resourceError.Play();
                    }
                }
            }
            if (Units1ToBuild > 0)
            {
                unit1BuildTime--;
                if (unit1BuildTime == 0)
                {
                    game.createHumanSprite(0, spriteRectangle.X, spriteRectangle.Y, unitProducedWaypoint.X, unitProducedWaypoint.Y);
                    Units1ToBuild--;
                    unit1BuildTime = 420;
                }
            }
            if (Units2ToBuild > 0)
            {
                unit2BuildTime--;
                if (unit2BuildTime == 0)
                {
                    game.createHumanSprite(1, spriteRectangle.X, spriteRectangle.Y, unitProducedWaypoint.X, unitProducedWaypoint.Y);
                    Units2ToBuild--;
                    unit2BuildTime = 480;
                }
            }
            base.Update(game);
        }
    }
    public class HumanLandFactory : HumanBuilding
    {
        public HumanLandFactory(
            Texture2D inSpriteTexture,
            Texture2D inTankTex,
            Texture2D inDistex,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound)
            : base(inSpriteTexture, inTankTex, inDistex, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
             explosionTex,
             explosionSound)
        {
            indicator = "landfact";
            spriteTexture = inSpriteTexture;
            initialX = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
            unit1BuildTime = 300;
            unit2BuildTime = 600;
            unit1buildcost = 40;
            unit2buildcost = 200;
        }
        public override void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") { unitProducedWaypoint.Y -= movedegree; }
                if (direction == "down") { unitProducedWaypoint.Y += movedegree; }
                //moves left/right
                if (direction == "left") { unitProducedWaypoint.X -= movedegree; }
                if (direction == "right") { unitProducedWaypoint.X += movedegree; }
            }
            base.move(direction, jump, jumpx, jumpy);
        }
        public override void Update(Gebieter game)
        {

            if (state == BaseBuildingState.selected)
            {
                if (game.clicked)
                {
                    if (game.CursorRec.Intersects(UnitButton1Rec) && game.CursorRec.X < UnitButton1Rec.Right)
                    {
                        if (game.resouces > unit1buildcost)
                        {
                            Units1ToBuild++;
                            game.resouces -= unit1buildcost;
                        }
                        else game.resourceError.Play();
                    }
                    if (game.CursorRec.Intersects(UnitButton2Rec) && game.CursorRec.X > UnitButton1Rec.X)
                    {
                        if (game.resouces > unit2buildcost)
                        {
                            Units2ToBuild++;
                            game.resouces -= unit2buildcost;
                        }
                        else game.resourceError.Play();
                    }
                }
            }
            if (Units1ToBuild > 0)
            {
                unit1BuildTime--;
                if (unit1BuildTime == 0)
                {
                    game.createHumanSprite(2, spriteRectangle.X, spriteRectangle.Bottom, unitProducedWaypoint.X, unitProducedWaypoint.Y);
                    Units1ToBuild--;
                    unit1BuildTime = 300;
                }
            }
            if (Units2ToBuild > 0)
            {
                unit2BuildTime--;
                if (unit2BuildTime == 0)
                {
                    game.createHumanSprite(7, spriteRectangle.X, spriteRectangle.Bottom, unitProducedWaypoint.X, unitProducedWaypoint.Y);
                    Units2ToBuild--;
                    unit2BuildTime = 600;
                }
            }
            base.Update(game);
        }
    }
    public class HumanTurret : HumanBuilding
    {
        SoundEffect Hit;

        Rectangle Range;
        Texture2D Rangeblank;

        float LaserRotation;
        Rectangle laserRec;
        Texture2D laserTex;

        public HumanTurret(
            Texture2D inSpriteTexture,
            Texture2D rangeBlank,
            Texture2D lasertex,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect lasersound)
            : base(inSpriteTexture, null, null, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
            explosionTex,
            explosionSound)
        {
            Hit = lasersound;
            indicator = "turret";
            spriteTexture = inSpriteTexture;
            initialY = inX;
            initialY = inY;
            Rangeblank = rangeBlank;
            X = (int)(initialX);
            Y = (int)(initialY);
            laserTex = lasertex;
        }

        public override void move(string direction, bool jump,float jumpx,float jumpy)
        {
            //moves up/down
            if (direction == "up") { laserRec.Y -= movedegree; }
            if (direction == "down") { laserRec.Y += movedegree; }
            //moves left/right
            if (direction == "left") { laserRec.X -= movedegree; }
            if (direction == "right") { laserRec.X += movedegree; }
            base.move(direction, jump,jumpx,jumpy);
        }

        public override void Update(Gebieter game)
        {
            Range = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width * 2)), (int)(spriteRectangle.Y - (spriteRectangle.Height * 2)), spriteRectangle.Width * 5, spriteRectangle.Height * 5);
            if (game.skipUpdate == false)
            {
                #region attacking
                //attacking switch

                foreach (baseSprite enemy in game.Enemies)
                {
                    if (Range.Intersects(enemy.spriteRectangle) && state != BaseBuildingState.Building && enemy.IsExploding == false && exploding == false && dead == false)
                    {
                        incombat = true;
                    }
                    else incombat = false;


                    if (incombat == true)
                    {
                        int xdis = spriteRectangle.X - enemy.spriteRectangle.X;
                        int ydis = spriteRectangle.Y - enemy.spriteRectangle.Y;

                        int zdis = (int)Math.Sqrt((ydis * ydis) + (xdis * xdis));

                        laserRec = new Rectangle(spriteRectangle.X + (spriteRectangle.Width / 2), spriteRectangle.Y + (spriteRectangle.Height / 2), zdis, (int)DisplayHeight / 100);

                        LaserRotation = (float)(Math.Atan2(ydis, xdis)) - (float)Math.PI;

                        enemy.UpdateHealth(-.5f);
                        Hit.Play();
                        break;
                    }
                }
                #endregion
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (state == BaseBuildingState.selected) spritebatch.Draw(Rangeblank, Range, new Color(100, 0, 0, 80));
            base.Draw(spritebatch);
            if (incombat && exploding == false && dead == false) spritebatch.Draw(laserTex, laserRec, laserRec, new Color(0, 183, 239), LaserRotation, new Vector2(0, 0), SpriteEffects.None, 0);
        }

    }

    public class AreaEnhancer : HumanBuilding
    {
        public Rectangle bluearea;
        bool gamebuilding;
        Texture2D blank;

        public AreaEnhancer(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D inblank,
            Texture2D explosionTex,
            SoundEffect explosionSound)
            : base(inSpriteTexture, null, null, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
            explosionTex,
            explosionSound)
        {
            indicator = "areaEnhancer";
            spriteTexture = inSpriteTexture;
            initialY = inX;
            initialY = inY;
            blank = inblank;
            X = (int)(initialX);
            Y = (int)(initialY);
        }

        public override void Update(Gebieter game)
        {
            bluearea = new Rectangle(spriteRectangle.X - (spriteRectangle.Width * 2), spriteRectangle.Y - (spriteRectangle.Height * 2), spriteRectangle.Width * 5, spriteRectangle.Height * 5);
            if (game.building == true)
            {
                gamebuilding = true;
            }
            else gamebuilding = false;
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (gamebuilding == true || state == BaseBuildingState.selected) spritebatch.Draw(blank, bluearea, new Color(0, 50, 100, 80));
            base.Draw(spritebatch);
        }
    }
    public class TempleOrResearch : AreaEnhancer
    {
        public TempleOrResearch(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D inblank,
            Texture2D explosionTex,
            SoundEffect explosionSound)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime, inblank,
            explosionTex,
            explosionSound)
        {
            indicator = "temple";
            spriteTexture = inSpriteTexture;
            initialX = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
        }
    }
    //ai
    public class Aibuilding : baseSprite
    {
        protected int buildprogression;
        protected int buildFinishTime;

        protected Random randunitspawn;
        protected int randunitTimer;
        protected Rectangle unitProducedWaypoint;
        protected int unitToProducetime;
        protected int unitProduced;

        public Rectangle NetRange;
        protected Texture2D netRangeTex;
        protected float netrotation;
        public int netFrequency;
        public int connectionFrequency;

        public Aibuilding(
            Texture2D inSpriteTexture,
            Texture2D inUnit1,
            Texture2D inUnit2,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound, Texture2D range, int confreq)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, explosionTex, explosionSound)
        {
            connectionFrequency = confreq;
            netRangeTex = range;
            spriteTexture = inSpriteTexture;
            initialX = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
            buildFinishTime = inbuildtime;
            unitToProducetime = 360;
            movedegree = 4;
        }

        #region state setup

        public enum BaseBuildingState
        {
            Building,
            Built
        }

        public BaseBuildingState state;

        #endregion

        public override void Update(Gebieter game)
        {
            unitProducedWaypoint.X = (int)(X + 10);
            unitProducedWaypoint.Y = (int)(X + 10);

            NetRange = new Rectangle(spriteRectangle.X - (int)(spriteRectangle.Width * 2f), spriteRectangle.Y - (int)(spriteRectangle.Height * 2f), spriteRectangle.Width * 5, spriteRectangle.Height * 5);
            //if building is in progress of being built
            if (buildprogression < buildFinishTime) state = BaseBuildingState.Building;

            #region state switch
            switch (state)
            {
                case BaseBuildingState.Building:
                    if (connected) buildprogression++;

                    if (buildprogression <= (buildFinishTime / 3)) { SetRow(4); }
                    if (buildprogression > (buildFinishTime / 3) && buildprogression <= ((buildFinishTime / 3) * 2)) { SetRow(2); }
                    if (buildprogression > (buildFinishTime / 3) * 2) { SetRow(3); }

                    //building has been built
                    if (buildprogression == buildFinishTime) { state = BaseBuildingState.Built; SetRow(0); }
                    break;
                case BaseBuildingState.Built:
                    break;
            }
            #endregion

            foreach (baseSprite human in game.Humans)
            {
                if (human.spriteRectangle.Intersects(NetRange))
                {
                    incombat = true;
                    break;
                }
                else
                {
                    incombat = false;
                }
            }

            if (game.skipUpdate == false)
            {
                #region connected

                if (connected && indicator != "network")
                {
                    foreach (Aibuilding node in game.EnemyBuildingList)
                    {
                        if (node.connected == false && NetRange.Intersects(node.spriteRectangle) && node.indicator != "network")
                        {
                            int xdis = spriteRectangle.X - node.spriteRectangle.X;
                            int ydis = spriteRectangle.Y - node.spriteRectangle.Y;
                            //length of tencical
                            int zdis = (int)Math.Sqrt((ydis * ydis) + (xdis * xdis));

                            netrotation = (float)Math.Atan2(ydis, xdis) - (float)Math.PI;

                            game.createConnection(spriteRectangle.X + (spriteRectangle.Width / 2), spriteRectangle.Y + (spriteRectangle.Height / 2), zdis, netrotation, netFrequency, node.connectionFrequency);

                            node.netFrequency = netFrequency;
                            node.connected = true;
                        }
                    }
                }
                #endregion
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(netRangeTex, NetRange, Color.Red);
            if (connected)
            {
                base.Draw(spritebatch);
            }
            else
            {
                if (exploding || dead)
                {
                    spritebatch.Draw(explodeTexture, spriteRectangle,
                                     explodeSourceRect, Color.White);
                }
                else spritebatch.Draw(spriteTexture, spriteRectangle, sourceRec, Color.Gray);
            }
        }
    }

    public class AIspawner : Aibuilding
    {

        Random ranspawner;
        int unitspawntime;
        public AIspawner(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound, Texture2D range, int confreq)
            : base(inSpriteTexture, null, null, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
            explosionTex,
            explosionSound, range, confreq)
        {
            indicator = "aispawner";
            initialY = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
            unitProducedWaypoint.X = (spriteRectangle.X - spriteRectangle.Width);
            unitProducedWaypoint.Y = spriteRectangle.Y;
            unitProduced = 0;
            spriteTexture = inSpriteTexture;
            ranspawner = new Random();
        }

        public override void Update(Gebieter game)
        {
            if (connected)
            {
                if (unitspawntime == 0)
                {
                    unitspawntime = ranspawner.Next(1200, 2100);
                    //if even #, spawn seeder
                    if (unitspawntime%2 == 0) game.createAISprite(0, spriteRectangle.X, spriteRectangle.Y, spriteRectangle.X - spriteRectangle.Width, spriteRectangle.Y - spriteRectangle.Height);
                    else game.createAISprite(2, spriteRectangle.X, spriteRectangle.Y, spriteRectangle.X - spriteRectangle.Width, spriteRectangle.Y - spriteRectangle.Height);
                }
                else unitspawntime--;
            }
            base.Update(game);
        }
    }
    public class AIturret : Aibuilding
    {
        SoundEffect Hit;
        Rectangle Range;
        bool fired;
        int reloadtime;
        Rectangle laserRec;
        Texture2D laserTex;
        float LaserRotation;

        public AIturret(
            Texture2D inSpriteTexture,
            Texture2D lasertex,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect Lasersound, Texture2D range, int confreq)
            : base(inSpriteTexture, null, null, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
            explosionTex,
            explosionSound, range, confreq)
        {
            indicator = "aiturret";
            Hit = Lasersound;
            laserTex = lasertex;
            spriteTexture = inSpriteTexture;
            initialY = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
        }

        public override void move(string direction, bool jump,float jumpx,float jumpy)
        {
            if (direction == "up") laserRec.Y -= movedegree;
            if (direction == "down") laserRec.Y += movedegree;

            if (direction == "left") laserRec.X -= movedegree;
            if (direction == "right") laserRec.X += movedegree;
            base.move(direction, jump,jumpx,jumpy);
        }

        public override void Update(Gebieter game)
        {
            if (connected && state == BaseBuildingState.Built && game.skipUpdate == false)
            {
                #region attacking
                //attacking switch
                Range = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width * 3)), (int)(spriteRectangle.Y - (spriteRectangle.Height * 3)), spriteRectangle.Width * 7, spriteRectangle.Height * 7);

                foreach (baseSprite human in game.Humans)
                {
                    if (Range.Intersects(human.spriteRectangle) && human.IsExploding == false && human.inTransport == false)
                    {
                        incombat = true;
                    }
                    else
                    {
                        incombat = false;
                    }

                    if (incombat == true)
                    {
                        int xdis = spriteRectangle.X - human.spriteRectangle.Center.X;
                        int ydis = spriteRectangle.Y - human.spriteRectangle.Center.Y;

                        int zdis = (int)Math.Sqrt((ydis * ydis) + (xdis * xdis));

                        laserRec = new Rectangle(spriteRectangle.X + (int)(spriteRectangle.Width / 2.5f), spriteRectangle.Y + (int)(spriteRectangle.Height / 2.5f), zdis, (int)DisplayHeight / 50);

                        LaserRotation = (float)(Math.Atan2(ydis, xdis)) - (float)Math.PI;

                        //play put on through the timer
                        if (reloadtime == 10)
                        {
                            human.UpdateHealth(-5);
                            Hit.Play();
                        }
                        if (reloadtime <= 10)
                        {
                            fired = true;
                        }
                        else fired = false;

                        //resets timer
                        if (reloadtime == 0)
                        {
                            reloadtime = 180;
                        }
                        else
                        {
                            reloadtime--;
                        }
                        break;
                    }
                }
                #endregion
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
            if (incombat && exploding == false && dead == false && fired)
            {
                spritebatch.Draw(laserTex, laserRec, null, Color.White, LaserRotation, new Vector2(0, 0), SpriteEffects.None, 0);
            }
        }
    }
    public class AIseed : Aibuilding
    {
        SpriteFont temp;

        public AIseed(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound, Texture2D range, int confreq, SpriteFont Temp)
            : base(inSpriteTexture, null, null, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
            explosionTex,
            explosionSound, range, confreq)
        {
            indicator = "aiseed";
            temp = Temp;
            unitProducedWaypoint.X = (spriteRectangle.X - spriteRectangle.Width);
            unitProducedWaypoint.Y = spriteRectangle.Y;
            unitProduced = 1;
            spriteTexture = inSpriteTexture;
            initialY = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
        }

        public override void Update(Gebieter game)
        {
            randunitspawn = new Random();
            if (connected)
            {
                if (randunitTimer == 0)
                {
                    game.createAISprite(1, spriteRectangle.X, spriteRectangle.Y, spriteRectangle.X - spriteRectangle.Width, spriteRectangle.Y);
                    randunitTimer = randunitspawn.Next(600, 2400);
                }
                else
                {
                    randunitTimer--;
                }
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
            //spritebatch.DrawString(temp, "" + connected, new Vector2(spriteRectangle.Right - 5, spriteRectangle.Y), Color.Red);
            //spritebatch.DrawString(temp, "" + passedon, new Vector2(spriteRectangle.X, spriteRectangle.Y), Color.Blue);
        }

    }
    public class AINetwork : Aibuilding
    {
        public AINetwork(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inX,
            float inY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int inbuildtime,
            Texture2D explosionTex,
            SoundEffect explosionSound, Texture2D range, int netfreq, int confreq)
            : base(inSpriteTexture, null, null, widthFactor, inDisplayWidth, inDisplayHieght, inX, inY, inFrameWidth, inFrameHeight, inhealth, inbuildtime,
            explosionTex,
            explosionSound, range, confreq)
        {
            connected = true;
            netFrequency = netfreq;
            indicator = "network";
            spriteTexture = inSpriteTexture;
            initialY = inX;
            initialY = inY;
            X = (int)(initialX);
            Y = (int)(initialY);
        }

        public override void Update(Gebieter game)
        {
            if (game.skipUpdate == false)
            {
                foreach (Aibuilding building in game.EnemyBuildingList)
                {
                    if (building.indicator != "network" && building.connected == false && building.spriteRectangle.Intersects(NetRange))
                    {
                        int xdis = spriteRectangle.X - building.spriteRectangle.X;
                        int ydis = spriteRectangle.Y - building.spriteRectangle.Y;
                        //length of tencical
                        int zdis = (int)Math.Sqrt((ydis * ydis) + (xdis * xdis));

                        netrotation = (float)Math.Atan2(ydis, xdis) - (float)Math.PI;

                        game.createConnection(spriteRectangle.X + (spriteRectangle.Width / 2), spriteRectangle.Y + (spriteRectangle.Height / 2), zdis, netrotation, netFrequency, building.connectionFrequency);

                        building.netFrequency = netFrequency;
                        building.connected = true;
                    }
                }
            }
            base.Update(game);
        }
    }
    public class NetTentical
    {
        public Rectangle spriteRec;
        Texture2D spriteTex;
        float rotation;
        public int freQuency;
        public bool lost;
        public bool notlost;
        public int designatedconnection;

        SpriteFont Temp;
        public NetTentical(
            Texture2D inSpriteTexture,
            int length,
            float inDisplayHieght,
            int inX,
            int inY,
            float inrotation,
            int frequency,
            int designatedCon, SpriteFont temp)
        {
            Temp = temp;
            designatedconnection = designatedCon;
            freQuency = frequency;
            rotation = inrotation;
            spriteTex = inSpriteTexture;
            spriteRec = new Rectangle(inX, inY, length, (int)inDisplayHieght / 50);
        }

        /// <summary>
        /// moves units/screen to the scrolling
        /// </summary>
        /// <param name="direction">true = up/down false = left/right </param>
        /// <param name="degree">amount to be added/minused</param>
        public virtual void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") spriteRec.Y -= 4;
                if (direction == "down") spriteRec.Y += 4;
                //moves left/right
                if (direction == "left") spriteRec.X -= 4;
                if (direction == "right") spriteRec.X += 4;
            }
            else
            {
                spriteRec.X = (int)jumpx;
                spriteRec.Y = (int)jumpy;
            }
        }

        public void drawtentical(SpriteBatch spritebatch)
        {
            spritebatch.Draw(spriteTex, spriteRec, new Rectangle(0, 0, 100, 25), Color.White, rotation, new Vector2(0, 0), SpriteEffects.None, 0);
            //spritebatch.DrawString(Temp, "" + designatedconnection, new Vector2(spriteRec.X - 50, spriteRec.Y + 50), Color.Yellow);
        }
    }

    #endregion

    #region Units
    //human
    public class BaseUnit : baseSprite
    {
        //protected SpriteFont temp;

        protected SoundEffect Hit;
        public float rotation;
        public Rectangle waypoint;
        protected float redzone;
        protected float tourqe;
        protected float XDistance;
        protected float YDistance;
        protected Rectangle bulletRec;
        protected Texture2D bulletTex;

        public bool setStrike;
        public Rectangle strikearea;

        protected Rectangle Range;
        protected Texture2D RangeTex;
        protected bool showran;
        protected bool showhelt;

        //stuff that needs to be acsessable to multiple things but not all
        public bool UsingTransport;
        public int transportnumber;

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public BaseUnit(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWaypointX,
            int firstWAypointY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrangetex)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth,
            explosionTex,
            explosionSound)
        {
            tourqe = 100;
            connected = true;
            Hit = hit;
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            RangeTex = inrangetex;
            waypoint = new Rectangle(firstWaypointX, firstWAypointY, 10, 10);
            state = BaseUnitState.moving;
        }

        #region state setup

        public enum BaseUnitState
        {
            stationary,
            selected,
            moving
        }

        public BaseUnitState state;

        #endregion

        public override void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                if (direction == "up") { waypoint.Y -= movedegree; strikearea.Y -= 4; bulletRec.Y -= 4; }
                if (direction == "down") { waypoint.Y += movedegree; strikearea.Y += 4; bulletRec.Y += 4; }

                if (direction == "left") { waypoint.X -= movedegree; strikearea.X -= 4; bulletRec.X -= 4; }
                if (direction == "right") { waypoint.X += movedegree; strikearea.X += 4; bulletRec.X += 4; }
            }
            else
            {
                waypoint.X = (int)(jumpx + (waypoint.X - spriteRectangle.X));
                waypoint.Y = (int)(jumpy + (waypoint.Y - spriteRectangle.Y));
                strikearea.X = (int)(jumpx + (strikearea.X - spriteRectangle.X));
                strikearea.Y = (int)(jumpy + (strikearea.Y - spriteRectangle.Y));
                bulletRec.X = (int)(jumpx + (bulletRec.X - spriteRectangle.X));
                bulletRec.X = (int)(jumpy + (bulletRec.Y - spriteRectangle.Y));
            }
            base.move(direction, jump,jumpx,jumpy);
        }

        public override void Update(Gebieter game)
        {
            KeyboardState keys = Keyboard.GetState();
            showran = game.showranges;
            showhelt = game.showHealths;

            #region state changing
            //if it's intersected
            if (spriteRectangle.Intersects(game.DragBarRec) && game.selecting == true)
            {
                //and is completely in the box
                if (spriteRectangle.X > game.DragBarRec.X && spriteRectangle.Y > game.DragBarRec.Y)
                {
                    //then it's selcted
                    state = BaseUnitState.selected;
                }
            }
            if (game.clicked == true && game.CursorRec.X > spriteRectangle.X && game.CursorRec.X < spriteRectangle.Right
                && game.CursorRec.Y > spriteRectangle.Y && game.CursorRec.Y < spriteRectangle.Bottom && game.CursorRec.Intersects(game.HudRec) == false)
            {
                state = BaseUnitState.selected;
            }
            if (!game.CursorRec.Intersects(spriteRectangle) && !game.CursorRec.Intersects(game.miniMap)
                && game.clicked && state == BaseUnitState.selected) state = BaseUnitState.stationary;
            #endregion

            #region state
            switch (state)
            {
                case BaseUnitState.stationary:
                    SetRow(0);
                    break;
                case BaseUnitState.selected:
                    SetRow(1);
                    if (game.setWaypoint == true)
                    {
                        state = BaseUnitState.moving;
                    }
                    break;
                case BaseUnitState.moving:
                    if (indicator == "Disruptor") SetRow(2);
                    else SetRow(0);
                    XDistance = (spriteRectangle.X - waypoint.X) / tourqe;
                    YDistance = (spriteRectangle.Y - waypoint.Y) / tourqe;
                    if (XDistance > redzone) XDistance = redzone;
                    if (YDistance > redzone) YDistance = redzone;
                    if (XDistance < -redzone) XDistance = -redzone;
                    if (YDistance < -redzone) YDistance = -redzone;
                    if (spriteRectangle.Intersects(waypoint) == false)
                    {
                        rotation = (float)Math.Atan2(YDistance, XDistance);
                        X -= XDistance;
                        Y -= YDistance;
                    }
                    else  state = BaseUnitState.stationary;
                    break;
            }
            #endregion

            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (exploding)
            {
                spritebatch.Draw(explodeTexture, spriteRectangle,
                                 explodeSourceRect, Color.White);
            }
            else
            {
                Rectangle position = new Rectangle(spriteRectangle.X + (spriteRectangle.Width / 2), spriteRectangle.Y + (spriteRectangle.Height / 2), spriteRectangle.Width, spriteRectangle.Height);

                spritebatch.Draw(spriteTexture, position, sourceRec, Color.White, rotation, new Vector2(sourceRec.Width / 2, sourceRec.Height / 2), SpriteEffects.None, 0);

                if (showran) spritebatch.Draw(RangeTex, Range, new Color(255, 0, 0, 100));
                if (showhelt) spritebatch.Draw(bulletTex, new Rectangle((int)(spriteRectangle.X + (spriteRectangle.Width / 2)), (int)(spriteRectangle.Bottom + (DisplayHeight / 80)), (int)(health / 5), (int)DisplayHeight / 80), new Color(34, 177, 76));
            }
        }
    }

    public class HumanFLyer : BaseUnit
    {
        float LaserRotation;

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public HumanFLyer(
            Texture2D inSpriteTexture,
            Texture2D bullettex,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWaypointX,
            int firstWAypointY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWaypointX, firstWAypointY,
            explosionTex, explosionSound, hit, inrange)
        {
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            indicator = "flyer";
            redzone = 1.5f;
            movedegree = 4;
            bulletTex = bullettex;
        }

        public override void Update(Gebieter game)
        {
            #region air avoidance
            foreach (BaseUnit airunit in game.HumanAirList)
            {
                if (spriteRectangle.Intersects(airunit.spriteRectangle) && state != BaseUnitState.moving && spriteRectangle.Intersects(airunit.spriteRectangle) && airunit.state != BaseUnitState.moving)
                {
                    if (spriteRectangle.Center.X > airunit.spriteRectangle.Center.X)
                    {
                        X += .2f;
                    }
                    if (spriteRectangle.Center.X < airunit.spriteRectangle.Center.X)
                    {
                        X -= .2f;
                    }
                    if (spriteRectangle.Center.Y > airunit.spriteRectangle.Center.Y)
                    {
                        Y += .2f;
                    }
                    if (spriteRectangle.Center.Y < airunit.spriteRectangle.Center.Y)
                    {
                        Y -= .2f;
                    }
                }
            }
            #endregion
            if (game.skipUpdate == false)
            {
                #region attacking
                Range = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width)), (int)(spriteRectangle.Y - (spriteRectangle.Height)), spriteRectangle.Width * 3, spriteRectangle.Height * 3);

                //sets strike to designated area
                if (setStrike)
                {
                    foreach (baseSprite enemy in game.Enemies)
                    {
                        if (enemy.spriteRectangle.Intersects(strikearea) && enemy.IsExploding == false && enemy.dead == false || Range.Intersects(enemy.spriteRectangle) && enemy.IsExploding == false && enemy.dead == false)
                        {
                            enemy.incombat = true;
                            waypoint.X = enemy.spriteRectangle.Center.X;
                            waypoint.Y = enemy.spriteRectangle.Center.Y;
                            state = BaseUnitState.moving;

                            if (Range.Intersects(enemy.spriteRectangle))
                            {
                                state = BaseUnitState.stationary;
                                XDistance = spriteRectangle.X - waypoint.X;
                                YDistance = spriteRectangle.Y - waypoint.Y;
                                int zdis = (int)Math.Sqrt((YDistance * YDistance) + (XDistance * XDistance));

                                bulletRec = new Rectangle(spriteRectangle.X + (int)(spriteRectangle.Width / 2.5f), spriteRectangle.Y + (int)(spriteRectangle.Height / 2.5f), zdis, (int)DisplayHeight / 100);

                                LaserRotation = (float)(Math.Atan2(YDistance, XDistance)) - (float)Math.PI;
                                rotation = (float)Math.Atan2(YDistance, XDistance);

                                Hit.Play();
                                enemy.UpdateHealth(-.5f);
                                incombat = true;
                            }
                            break;
                        }
                        incombat = false;
                    }
                }
                else
                {
                    if (state != BaseUnitState.moving)
                    {
                        foreach (baseSprite enemy in game.Enemies)
                        {
                            if (Range.Intersects(enemy.spriteRectangle) && enemy.IsExploding == false && enemy.dead == false)
                            {
                                int xdis = spriteRectangle.X - enemy.spriteRectangle.Center.X;
                                int ydis = spriteRectangle.Y - enemy.spriteRectangle.Center.Y;
                                int zdis = (int)Math.Sqrt((ydis * ydis) + (xdis * xdis));

                                bulletRec = new Rectangle(spriteRectangle.X + (int)(spriteRectangle.Width / 2.5f), spriteRectangle.Y + (int)(spriteRectangle.Height / 2.5f), zdis, (int)DisplayHeight / 100);

                                LaserRotation = (float)(Math.Atan2(ydis, xdis)) - (float)Math.PI;
                                rotation = (float)Math.Atan2(ydis, xdis);

                                Hit.Play();
                                enemy.UpdateHealth(-1);
                                incombat = true;
                                break;
                            }
                            else incombat = false;
                        }
                    }

                }

                #endregion
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Rectangle shadow = new Rectangle(spriteRectangle.X + (spriteRectangle.Width), spriteRectangle.Y + spriteRectangle.Height, spriteRectangle.Width / 2, spriteRectangle.Height / 2);
            spritebatch.Draw(spriteTexture, shadow, new Rectangle(0, 0, 70, 70), new Color(0, 0, 0, 50), rotation, new Vector2(shadow.Width / 2, shadow.Height / 2), SpriteEffects.None, 0);
            
            if (incombat) spritebatch.Draw(bulletTex, bulletRec, bulletRec, new Color(0, 183, 239), LaserRotation, new Vector2(0, 0), SpriteEffects.None, 0);
            base.Draw(spritebatch);
        }
    }
    public class HumanTransport : BaseUnit
    {
        int numHolding;
        bool holding;
        bool dropping;

        Rectangle DropOffButton;
        Texture2D DropOffTex;
        bool disrupload;
        Texture2D disruptor;

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public HumanTransport(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            Texture2D dropoffTex,
            int tranNumber,
            int firstWaypointX,
            int firstWAypointY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange, Texture2D healthtex, Texture2D inbigload)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWaypointX, firstWAypointY,
            explosionTex, explosionSound, hit, inrange)
        {
            disruptor = inbigload;
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            indicator = "tran";
            DropOffTex = dropoffTex;
            transportnumber = tranNumber;
            redzone = 1.2f;
            movedegree = 4;
            bulletTex = healthtex;
        }

        public override void Update(Gebieter game)
        {
            DropOffButton = new Rectangle(spriteRectangle.Right, spriteRectangle.Y, (int)(DisplayWidth / 50), (int)(DisplayWidth / 50));

            #region air avoidance
            foreach (BaseUnit airunit in game.HumanAirList)
            {
                if (spriteRectangle.Intersects(airunit.spriteRectangle) && state != BaseUnitState.moving || spriteRectangle.Intersects(airunit.spriteRectangle) && airunit.state != BaseUnitState.moving)
                {
                    if (spriteRectangle.Center.X > airunit.spriteRectangle.Center.X)
                    {
                        X += .2f;
                    }
                    if (spriteRectangle.Center.X < airunit.spriteRectangle.Center.X)
                    {
                        X -= .2f;
                    }
                    if (spriteRectangle.Center.Y > airunit.spriteRectangle.Center.Y)
                    {
                        Y +=.2f;
                    }
                    if (spriteRectangle.Center.Y < airunit.spriteRectangle.Center.Y)
                    {
                        Y -= .2f;
                    }
                }
            }
            #endregion

            #region transportation
            //set the tansport's pickup / drop off uses
            foreach (BaseUnit tank in game.HumanLandList)
            {
                //tank moves to transport
                if (tank.state == BaseUnitState.selected && game.clicked == true && spriteRectangle.Intersects(game.CursorRec) && numHolding < 15)
                {
                    holding = true;
                    tank.waypoint = spriteRectangle;
                    tank.UsingTransport = true;
                    tank.state = BaseUnitState.moving;
                }
                //unit gets in transport
                if (holding == true && tank.spriteRectangle.Intersects(spriteRectangle) && tank.UsingTransport == true && tank.state == BaseUnitState.moving && numHolding < 15)
                {
                    if (tank.indicator == "tank") numHolding++;
                    if (tank.indicator == "Disruptor") { numHolding += 5; disrupload = true; }
                    tank.transportnumber = transportnumber;
                    tank.inTransport = true;
                    tank.state = BaseUnitState.stationary;
                }
                if (state == BaseUnitState.selected)
                {
                    //unit is dropped from transport
                    if (game.clicked == true && game.CursorRec.Intersects(DropOffButton) && game.CursorRec.Top > DropOffButton.Top)
                    {
                        dropping = true;
                    }
                    else dropping = false;

                    if (dropping == true && tank.UsingTransport == true && tank.transportnumber == transportnumber)
                    {
                        numHolding = 0;
                        holding = false;
                        tank.UsingTransport = false;
                        tank.inTransport = false;
                        tank.X = spriteRectangle.X;
                        tank.Y = spriteRectangle.Y;
                        disrupload = false;
                    }
                    if (!game.CursorRec.Intersects(spriteRectangle) && !game.CursorRec.Intersects(game.miniMap)
                         && game.clicked && state == BaseUnitState.selected && !game.CursorRec.Intersects(DropOffButton)) state = BaseUnitState.stationary;
                }
            }
            #endregion

            //kills all intransport units if their transport dies
            if (exploding)
            {
                foreach (BaseUnit load in game.HumanLandList)
                {
                    if (load.transportnumber == transportnumber) load.dead = true;
                }
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (disrupload == false)
            {
                //shadow
                Rectangle shadow = new Rectangle(spriteRectangle.X + (spriteRectangle.Width), spriteRectangle.Y + spriteRectangle.Height, spriteRectangle.Width / 2, spriteRectangle.Height / 2);
                spritebatch.Draw(spriteTexture, shadow, new Rectangle(0, 0, 80, 80), new Color(0, 0, 0, 50), rotation, new Vector2(shadow.Width / 2, shadow.Height / 2), SpriteEffects.None, 0);
            }
            else
            {
                //shadow
                Rectangle shadow = new Rectangle(spriteRectangle.X + (spriteRectangle.Width), spriteRectangle.Y + spriteRectangle.Height, spriteRectangle.Width / 2, spriteRectangle.Height / 2);
                spritebatch.Draw(disruptor, shadow, new Rectangle(0, 0, 80, 80), new Color(0, 0, 0, 50), rotation, new Vector2(shadow.Width / 2, shadow.Height / 2), SpriteEffects.None, 0);
                //shows disruptor
                Rectangle position = new Rectangle(spriteRectangle.X + (spriteRectangle.Width / 2), spriteRectangle.Y + (spriteRectangle.Height / 2), (int)(spriteRectangle.Width/1.4f), (int)(spriteRectangle.Height/1.4f));
                spritebatch.Draw(disruptor, position, new Rectangle(0, 0, 100, 100), Color.White, rotation, new Vector2(sourceRec.Width / 2, sourceRec.Height / 2), SpriteEffects.None, 0);
            }
            base.Draw(spritebatch);
            if (numHolding > 0)
            {
                spritebatch.Draw(bulletTex, new Rectangle(spriteRectangle.X, spriteRectangle.Bottom + (int)(DisplayWidth / 100), 60, (int)(DisplayWidth / 100)), Color.Gray);
                spritebatch.Draw(bulletTex, new Rectangle(spriteRectangle.X, spriteRectangle.Bottom + (int)(DisplayWidth / 100), numHolding * 4, (int)(DisplayWidth / 100)), Color.Blue);
                if (state == BaseUnitState.selected)
                {
                    spritebatch.Draw(DropOffTex, DropOffButton, Color.White);
                }
            }
        }
    }
    public class HumanTank : BaseUnit
    {
        float LaserRotation;
        bool fired;
        int reloadtime;

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public HumanTank(
            Texture2D inSpriteTexture,
            Texture2D bullettex,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWaypointX,
            int firstWAypointY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWaypointX, firstWAypointY,
            explosionTex, explosionSound, hit, inrange)
        {
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            indicator = "tank";
            movedegree = 4;
            redzone = 1;
            bulletTex = bullettex;
        }

        public override void Update(Gebieter game)
        {
            if (UsingTransport == false)
            {
                #region building avoidance

                foreach (HumanBuilding building in game.HumanBuildingList)
                {
                    if (spriteRectangle.Intersects(building.spriteRectangle) && waypoint.Intersects(building.spriteRectangle))
                    {
                        state = BaseUnitState.stationary;
                        waypoint.X = (int)building.X - waypoint.Width;
                        waypoint.Y = (int)building.Y - waypoint.Height;
                    }

                    if (spriteRectangle.Intersects(building.spriteRectangle) && inTransport == false)
                    {
                        if (spriteRectangle.Center.X > building.spriteRectangle.Center.X)
                        {
                            X += .5f;
                        }
                        if (spriteRectangle.Center.X < building.spriteRectangle.Center.X)
                        {
                            X -= .5f;
                        }
                        if (spriteRectangle.Center.Y > building.spriteRectangle.Center.Y)
                        {
                            Y += .5f;
                        }
                        if (spriteRectangle.Center.Y < building.spriteRectangle.Center.Y)
                        {
                            Y -= .5f;
                        }

                    }

                }
                #endregion

                #region tank avoidance

                foreach (BaseUnit tank in game.HumanLandList)
                {
                    if (spriteRectangle.Intersects(tank.spriteRectangle) && inTransport == false && tank.inTransport == false)
                    {
                        if (spriteRectangle.Center.X > tank.spriteRectangle.Center.X)
                        {
                            X += .5f;
                        }
                        if (spriteRectangle.Center.X < tank.spriteRectangle.Center.X)
                        {
                            X -= .5f;
                        }
                        if (spriteRectangle.Center.Y > tank.spriteRectangle.Center.Y)
                        {
                            Y += .5f;
                        }
                        if (spriteRectangle.Center.Y < tank.spriteRectangle.Center.Y)
                        {
                            Y -= .5f;
                        }

                    }
                }

                #endregion
                if (game.skipUpdate == false)
                {
                    #region attacking
                    Range = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width * 3)), (int)(spriteRectangle.Y - (spriteRectangle.Height * 3)), spriteRectangle.Width * 7, spriteRectangle.Height * 7);
                    if (setStrike)
                    {
                        foreach (baseSprite enemy in game.Enemies)
                        {
                            if (enemy.spriteRectangle.Intersects(strikearea) && enemy.IsExploding == false && enemy.dead == false || Range.Intersects(enemy.spriteRectangle) && enemy.IsExploding == false && enemy.dead == false)
                            {
                                enemy.incombat = true;
                                waypoint.X = enemy.spriteRectangle.Center.X;
                                waypoint.Y = enemy.spriteRectangle.Center.Y;
                                state = BaseUnitState.moving;

                                if (Range.Intersects(enemy.spriteRectangle))
                                {
                                    state = BaseUnitState.stationary;
                                    XDistance = spriteRectangle.X - waypoint.X;
                                    YDistance = spriteRectangle.Y - waypoint.Y;
                                    int zdis = (int)Math.Sqrt((YDistance * YDistance) + (XDistance * XDistance));

                                    bulletRec = new Rectangle(spriteRectangle.X + (int)(spriteRectangle.Width / 2.5f), spriteRectangle.Y + (int)(spriteRectangle.Height / 2.5f), zdis, (int)DisplayHeight / 100);

                                    LaserRotation = (float)(Math.Atan2(YDistance, XDistance)) - (float)Math.PI;
                                    rotation = (float)Math.Atan2(YDistance, XDistance);

                                    if (reloadtime == 0)
                                    {
                                        enemy.UpdateHealth(-20);
                                        fired = true;
                                        Hit.Play();
                                        reloadtime = 180;
                                    }
                                    else
                                    {
                                        reloadtime--;
                                        fired = false;
                                    }
                                    incombat = true;
                                }
                                break;
                            }
                            incombat = false;
                        }
                    }
                    else
                    {
                        if (state != BaseUnitState.moving)
                        {
                            foreach (baseSprite enemy in game.Enemies)
                            {
                                if (Range.Intersects(enemy.spriteRectangle) && enemy.IsExploding == false && enemy.dead == false)
                                {
                                    int xdis = spriteRectangle.X - enemy.spriteRectangle.Center.X;
                                    int ydis = spriteRectangle.Y - enemy.spriteRectangle.Center.Y;
                                    int zdis = (int)Math.Sqrt((ydis * ydis) + (xdis * xdis));

                                    bulletRec = new Rectangle(spriteRectangle.X + (int)(spriteRectangle.Width / 2.5f), spriteRectangle.Y + (int)(spriteRectangle.Height / 2.5f), zdis, (int)DisplayHeight / 100);

                                    LaserRotation = (float)(Math.Atan2(ydis, xdis)) - (float)Math.PI;
                                    rotation = (float)Math.Atan2(ydis, xdis);

                                    if (reloadtime == 0)
                                    {
                                        enemy.UpdateHealth(-20);
                                        fired = true;
                                        Hit.Play();
                                        reloadtime = 180;
                                    }
                                    else
                                    {
                                        reloadtime--;
                                        fired = false;
                                    }
                                    incombat = true;
                                    break;
                                }
                                else incombat = false;
                            }
                        }

                    }
                    #endregion
                }
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (inTransport == false)
            {
                if (incombat)
                {
                    if (fired) spritebatch.Draw(bulletTex, bulletRec, bulletRec, Color.Yellow, LaserRotation, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                base.Draw(spritebatch);
            }
        }
    }
    public class Disruptor : BaseUnit
    {
        Rectangle target;
        int reloadtime;
        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public Disruptor(
            Texture2D inSpriteTexture,
            Texture2D bullettex,
            Texture2D insmoke,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWaypointX,
            int firstWAypointY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWaypointX, firstWAypointY,
            explosionTex, explosionSound, hit, inrange)
        {
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            indicator = "Disruptor";
            movedegree = 4;
            redzone = .8f;
            bulletTex = bullettex;
        }

        public override void Update(Gebieter game)
        {
            if (inTransport == false)
            {
                #region building avoidance

                foreach (HumanBuilding building in game.HumanBuildingList)
                {
                    if (spriteRectangle.Intersects(building.spriteRectangle) && waypoint.Intersects(building.spriteRectangle))
                    {
                        state = BaseUnitState.stationary;
                        waypoint.X = (int)building.X - waypoint.Width;
                        waypoint.Y = (int)building.Y - waypoint.Height;
                    }

                    if (spriteRectangle.Intersects(building.spriteRectangle) && inTransport == false)
                    {
                        if (spriteRectangle.Center.X > building.spriteRectangle.Center.X)
                        {
                            X += .5f;
                        }
                        if (spriteRectangle.Center.X < building.spriteRectangle.Center.X)
                        {
                            X -= .5f;
                        }
                        if (spriteRectangle.Center.Y > building.spriteRectangle.Center.Y)
                        {
                            Y += .5f;
                        }
                        if (spriteRectangle.Center.Y < building.spriteRectangle.Center.Y)
                        {
                            Y -= .5f;
                        }

                    }

                }
                #endregion

                #region tank avoidance

                foreach (BaseUnit tank in game.HumanLandList)
                {
                    if (spriteRectangle.Intersects(tank.spriteRectangle) && inTransport == false && tank.inTransport == false)
                    {
                        if (spriteRectangle.Center.X > tank.spriteRectangle.Center.X)
                        {
                            X += .5f;
                        }
                        if (spriteRectangle.Center.X < tank.spriteRectangle.Center.X)
                        {
                            X -= .5f;
                        }
                        if (spriteRectangle.Center.Y > tank.spriteRectangle.Center.Y)
                        {
                            Y += .5f;
                        }
                        if (spriteRectangle.Center.Y < tank.spriteRectangle.Center.Y)
                        {
                            Y -= .5f;
                        }

                    }
                }

                #endregion
                if (game.skipUpdate == false)
                {
                    #region disrupting
                    Range = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width * 4)), (int)(spriteRectangle.Y - (spriteRectangle.Height * 4)), spriteRectangle.Width * 9, spriteRectangle.Height * 9);
                    //missles
                    if (reloadtime == 0)
                    {
                        foreach (baseSprite ene in game.Enemies)
                        {
                            if (ene.spriteRectangle.Intersects(Range) && ene.IsExploding == false && ene.dead == false)
                            {
                                ene.incombat = true;

                                target = ene.spriteRectangle;
                                game.launchMissile(spriteRectangle.Center.X, spriteRectangle.Center.Y, target, (int)(DisplayWidth / 40), (int)(DisplayWidth / 140), "network");
                                game.launchMissile(spriteRectangle.X, spriteRectangle.Center.Y, target, (int)(DisplayWidth / 40), (int)(DisplayWidth / 140), "network");
                                game.launchMissile(spriteRectangle.Center.X, spriteRectangle.Y, target, (int)(DisplayWidth / 40), (int)(DisplayWidth / 140), "network");
                                reloadtime = 200;
                                break;
                            }
                        }
                    }
                    else reloadtime--;
                    #endregion
                }
                base.Update(game);
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (inTransport == false) base.Draw(spritebatch);
        }
    }

    //ai
    public class Aiunit : baseSprite
    {
        public float rotation;
        public Rectangle waypoint;
        public Rectangle followWaypoint;
        protected float redzone;
        protected float XDistance;
        protected float YDistance;

        protected SoundEffect Hit;
        protected Rectangle bulletRec;
        protected Texture2D bulletTex;
        protected int damage;

        protected Rectangle Range;
        protected Texture2D RangeTex;
        protected bool following;
        protected Rectangle followRange;

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public Aiunit(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWaypointX,
            int firstWAypointY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrangetex)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth,
            explosionTex,
            explosionSound)
        {
            connected = true;
            Hit = hit;
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            RangeTex = inrangetex;
            waypoint.X = firstWaypointX;
            waypoint.Y = firstWAypointY;
            waypoint.Width = spriteRectangle.Width;
            waypoint.Height = waypoint.Width;
            state = BaseUnitState.moving;
        }

        #region state setup

        public enum BaseUnitState
        {
            stationary,
            moving
        }

        public BaseUnitState state;

        #endregion

        public override void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                if (direction == "up") { waypoint.Y -= movedegree; bulletRec.Y -= movedegree; }
                if (direction == "down") { waypoint.Y += movedegree; bulletRec.Y += movedegree; }

                if (direction == "left") { waypoint.X -= movedegree; bulletRec.X -= movedegree; }
                if (direction == "right") { waypoint.X += movedegree; bulletRec.X += movedegree; }
            }
            else
            {
                waypoint.X = (int)(jumpx + (waypoint.X - spriteRectangle.X));
                waypoint.Y = (int)(jumpy + (waypoint.Y - spriteRectangle.Y));
                bulletRec.X = (int)(jumpx + (bulletRec.X - spriteRectangle.X));
                bulletRec.Y = (int)(jumpy + (bulletRec.Y - spriteRectangle.Y));
            }
            base.move(direction, jump, jumpx, jumpy);
        }

        public override void Update(Gebieter game)
        {

            if (game.skipUpdate == false)
            {
                Range = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width * 2)), (int)(spriteRectangle.Y - (spriteRectangle.Height * 2)), spriteRectangle.Width * 5, spriteRectangle.Height * 5);
                followRange = new Rectangle((int)(spriteRectangle.X - (spriteRectangle.Width * 3)), (int)(spriteRectangle.Y - (spriteRectangle.Height * 3)), spriteRectangle.Width * 7, spriteRectangle.Height * 7);

                #region defened base

                foreach (Aibuilding building in game.EnemyBuildingList)
                {
                    if (Range.Intersects(building.NetRange) && building.incombat)
                    {
                        waypoint = building.spriteRectangle;
                        state = BaseUnitState.moving;
                    }
                }

                #endregion

                #region state
                switch (state)
                {
                    case BaseUnitState.stationary:
                        SetRow(0);
                        break;
                    case BaseUnitState.moving:
                        SetRow(1);

                        if (incombat == false)
                        {
                            XDistance = (spriteRectangle.X - waypoint.X) / 100;
                            YDistance = (spriteRectangle.Y - waypoint.Y) / 100;
                            if (XDistance > redzone) XDistance = redzone;
                            if (YDistance > redzone) YDistance = redzone;
                            if (XDistance < -redzone) XDistance = -redzone;
                            if (YDistance < -redzone) YDistance = -redzone;

                            if (spriteRectangle.Intersects(waypoint) == false)
                            {
                                rotation = (float)Math.Atan2(YDistance, XDistance);

                                X -= XDistance;
                                Y -= YDistance;
                            }
                            else state = BaseUnitState.stationary;
                        }
                        break;
                }
                #endregion
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (exploding)
            {
                spritebatch.Draw(explodeTexture, spriteRectangle,
                                 explodeSourceRect, Color.White);
            }
            else
            {
                Rectangle position = new Rectangle(spriteRectangle.X + (spriteRectangle.Width / 2), spriteRectangle.Y + (spriteRectangle.Height / 2), spriteRectangle.Width, spriteRectangle.Height);

                spritebatch.Draw(spriteTexture, position, sourceRec, Color.White, rotation, new Vector2(sourceRec.Width / 2, sourceRec.Height / 2), SpriteEffects.None, 0);
            }
        }

    }

    public class AISeeder : Aiunit
    {
        int reloadtime;
        Texture2D seedTex;
        Rectangle seedtarget;
        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public AISeeder(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWayX,
            int firstWayY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange,
            Texture2D inseedtex)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWayX, firstWayY,
            explosionTex, explosionSound, hit, inrange)
        {
            indicator = "seeder";
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            seedTex = inseedtex;
            direction = 1;
            redzone = .4f;
            movedegree = 4;
        }

        public override void Update(Gebieter game)
        {
            #region swarm avoidance

            foreach (Aiunit swarm in game.EnemyLandList)
            {
                if (spriteRectangle.Intersects(swarm.spriteRectangle))
                {
                    if (spriteRectangle.Center.X > swarm.spriteRectangle.Center.X)
                    {
                        X += .5f;
                    }
                    if (spriteRectangle.Center.X < swarm.spriteRectangle.Center.X)
                    {
                        X -= .5f;
                    }
                    if (spriteRectangle.Center.Y > swarm.spriteRectangle.Center.Y)
                    {
                        Y += .5f;
                    }
                    if (spriteRectangle.Center.Y < swarm.spriteRectangle.Center.Y)
                    {
                        Y -= .5f;
                    }

                }
            }

            #endregion

            if (game.skipUpdate == false)
            {
                if (reloadtime == 0)
                {
                    foreach (baseSprite human in game.Humans)
                    {
                        if (human.spriteRectangle.Intersects(Range) && human.IsExploding == false && human.dead == false && human.indicator != "flyer" && human.indicator != "tran" && human.inTransport == false)
                        {
                            seedtarget = human.spriteRectangle;
                            game.launchMissile(spriteRectangle.Center.X, spriteRectangle.Center.Y, seedtarget, (int)(DisplayWidth / 30), (int)(DisplayWidth / 30), "seed");
                            reloadtime = 480;
                            break;
                        }
                    }
                }
                else reloadtime--;
            }
            base.Update(game);
        }
    }
    public class AISwarm : Aiunit
    {

        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public AISwarm(
            Texture2D inSpriteTexture,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWayX,
            int firstWayY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWayX, firstWayY,
            explosionTex, explosionSound, hit, inrange)
        {
            indicator = "swarm";
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            direction = 1;
            redzone = 1.5f;
            movedegree = 4;
        }

        public override void Update(Gebieter game)
        {
            #region swarm avoidance

            foreach (Aiunit swarm in game.EnemyLandList)
            {
                if (spriteRectangle.Intersects(swarm.spriteRectangle))
                {
                    if (spriteRectangle.Center.X > swarm.spriteRectangle.Center.X)
                    {
                        X += .5f;
                    }
                    if (spriteRectangle.Center.X < swarm.spriteRectangle.Center.X)
                    {
                        X -= .5f;
                    }
                    if (spriteRectangle.Center.Y > swarm.spriteRectangle.Center.Y)
                    {
                        Y += .5f;
                    }
                    if (spriteRectangle.Center.Y < swarm.spriteRectangle.Center.Y)
                    {
                        Y -= .5f;
                    }

                }
            }

            #endregion
            //strength boost
            damage = 10 + game.infectionDamageBoost;

            if (game.skipUpdate == false)
            {
                #region suicide

                foreach (baseSprite human in game.Humans)
                {
                    if (human.indicator != "flyer" && human.indicator != "tran" && human.inTransport == false && human.spriteRectangle.Intersects(Range))
                    {
                        incombat = true;
                        waypoint = human.spriteRectangle;
                        state = BaseUnitState.moving;

                        if (spriteRectangle.Intersects(human.spriteRectangle) && dead == false && exploding == false && human.dead == false && human.IsExploding == false)
                        {
                            human.UpdateHealth(-damage);
                            Explode();
                        }
                        else
                        {
                            rotation = (float)Math.Atan2(spriteRectangle.Y - waypoint.Y,spriteRectangle.X - waypoint.X);
                            X -= (float)(1.5 * Math.Cos(rotation));
                            Y -= (float)(1.5 * Math.Sign(rotation));
                        }
                        break;
                    }
                }

                #endregion
            }
            base.Update(game);
        }
    }
    public class AIDrifter : Aiunit
    {
        int reloadtime;
        Rectangle target;
        /// <summary>
        /// sets basic needs of all sprites for the humans
        /// </summary>
        /// <param name="inSpriteTexture">texture for sprite</param>
        /// <param name="tickstocrossscreen">cycles to move</param>
        /// <param name="inRectangle">rectangle for sprite </param>
        /// <param name="widthFactor">size ratio</param>
        /// <param name="inMinDisplayX">min screen width</param>
        /// <param name="inMaxDisplayX">max screen width</param>
        /// <param name="inMinDisplayY">min screen hieght</param>
        /// <param name="inMaxDisplayY">max screen hieght</param>
        /// <param name="inInitialX">starting X position</param>
        /// <param name="inInitialY">starting Y position</param>
        /// <param name="inFrameWidth">width of area to be drawn</param>
        /// <param name="inFrameHeight">height of area to be drawn</param>
        public AIDrifter(
            Texture2D inSpriteTexture,
            Texture2D bullettex,
            float widthFactor,
            float inDisplayWidth,
            float inDisplayHieght,
            float inInitialX,
            float inInitialY,
            int inFrameWidth,
            int inFrameHeight,
            int inhealth,
            int firstWayX,
            int firstWayY,
            Texture2D explosionTex,
            SoundEffect explosionSound,
            SoundEffect hit,
            Texture2D inrange)
            : base(inSpriteTexture, widthFactor, inDisplayWidth, inDisplayHieght, inInitialX, inInitialY, inFrameWidth, inFrameHeight, inhealth, firstWayX, firstWayY,
            explosionTex, explosionSound, hit, inrange)
        {
            indicator = "drifter";
            spriteTexture = inSpriteTexture;
            initialX = initialX;
            initialY = initialY;
            X = initialX;
            Y = initialY;
            redzone = 1.2f;
            movedegree = 4;
        }

        public override void Update(Gebieter game)
        {
            #region swarm avoidance

            foreach (Aiunit swarm in game.EnemyAirList)
            {
                if (spriteRectangle.Intersects(swarm.spriteRectangle))
                {
                    if (spriteRectangle.Center.X > swarm.spriteRectangle.Center.X)
                    {
                        X += .5f;
                    }
                    if (spriteRectangle.Center.X < swarm.spriteRectangle.Center.X)
                    {
                        X -= .5f;
                    }
                    if (spriteRectangle.Center.Y > swarm.spriteRectangle.Center.Y)
                    {
                        Y += .5f;
                    }
                    if (spriteRectangle.Center.Y < swarm.spriteRectangle.Center.Y)
                    {
                        Y -= .5f;
                    }

                }
            }

            #endregion
            //strength boost
            damage = 10 + game.infectionDamageBoost;

            if (game.skipUpdate == false)
            {
                if (reloadtime == 0)
                {
                    foreach (baseSprite human in game.Humans)
                    {
                        if (human.spriteRectangle.Intersects(Range) && human.IsExploding == false && human.dead == false)
                        {
                            target = human.spriteRectangle;
                            game.launchMissile(spriteRectangle.Center.X, spriteRectangle.Center.Y, target, (int)(DisplayWidth / 40), (int)(DisplayWidth / 140), "humans");
                            reloadtime = 100;
                            break;
                        }
                    }
                }
                else reloadtime--;
            }
            base.Update(game);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Rectangle shadow = new Rectangle(spriteRectangle.X + (spriteRectangle.Width), spriteRectangle.Y + spriteRectangle.Height, spriteRectangle.Width / 2, spriteRectangle.Height / 2);

            spritebatch.Draw(spriteTexture, shadow, new Rectangle(0, 0, 100, 100), new Color(0, 0, 0, 50), rotation, new Vector2(0, 0), SpriteEffects.None, 0);
            base.Draw(spritebatch);
        }
    }
    #endregion

    #region creative
    public class smoke
    {
        private int timer;
        private int Twotimer;
        public bool dispersed;
        public Rectangle smokeRec;
        private Texture2D smokeTex;
        private Color opaque;

        public smoke(
            Texture2D insmoke,
            int insize,
            int inX, int inY, int inDisperstime, Color incolor)
        {
            smokeRec = new Rectangle(inX, inY, insize, insize);
            smokeTex = insmoke;
            timer = inDisperstime;
            opaque = new Color(incolor.R, incolor.G, incolor.B, (inDisperstime + 60));
        }

        /// <summary>
        /// moves units/screen to the scrolling
        /// </summary>
        /// <param name="direction">true = up/down false = left/right </param>
        /// <param name="degree">amount to be added/minused</param>
        public void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") smokeRec.Y -= 4;
                if (direction == "down") smokeRec.Y += 4;
                //moves left/right
                if (direction == "left") smokeRec.X -= 4;
                if (direction == "right") smokeRec.X += 4;
            }
            else
            {
                smokeRec.X = (int)jumpx;
                smokeRec.Y = (int)jumpy;
            }
        }

        public void Update(Gebieter game)
        {
            if (timer == 0)
            {
                dispersed = true;
            }
            else
            {
                if (Twotimer == 0)
                {
                    smokeRec.X--;
                    smokeRec.Width += 2;
                    smokeRec.Y--;
                    smokeRec.Height += 2;
                    Twotimer = 10;
                }
                else Twotimer--;

                timer--;
                opaque.A--;
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(smokeTex, smokeRec, opaque);
        }
    }
    public class missiles
    {
        #region tex,rec,movement
        private Texture2D tex;
        private Vector2 Vec;
        public Rectangle rec;
        private int sizeX;
        private int sizeY;
        private Rectangle target;
        private float tardistanceX;
        private float tardistanceY;
        private float missleRotation;
        #endregion

        #region explosion
        public bool exploded;
        private Texture2D explodeTexture;
        private Rectangle explodeRec;
        private Rectangle explodeSourceRect;
        private int explodeTickCounter;
        private bool exploding;
        private SoundEffect explodeSound;
        #endregion

        public string Enemy;
         
        SpriteFont temp;
        public missiles(
            Texture2D intex,
            int x, int y,
            int insizeX, int insizeY,
            Rectangle intarget,
            SoundEffect inexplodesound, Texture2D inexplosionTex, string enemy)
        {
            Enemy = enemy;
            explodeTexture = inexplosionTex;
            explodeSound = inexplodesound;
            explodeSourceRect = new Rectangle(0, 0, 60, 60);
            tex = intex;
            target = intarget;
            sizeX = insizeX;
            sizeY = insizeY;
            Vec = new Vector2(x, y);
        }

        /// <summary>
        /// moves units/screen to the scrolling
        /// </summary>
        /// <param name="direction">true = up/down false = left/right </param>
        /// <param name="degree">amount to be added/minused</param>
        public void move(string direction, bool jump, float jumpx, float jumpy)
        {
            if (jump == false)
            {
                //moves up/down
                if (direction == "up") { Vec.Y -= 4; target.Y -= 4; explodeRec.Y -= 4; }
                if (direction == "down") { Vec.Y += 4; target.Y += 4; explodeRec.Y += 4; }
                //moves left/right
                if (direction == "left") { Vec.X -= 4; target.X -= 4; explodeRec.X -= 4; }
                if (direction == "right") { Vec.X += 4; target.X += 4; explodeRec.X += 4; }
            }
            else
            {
                Vec.X = jumpx;
                Vec.Y = jumpy;
                target.Y = (int)(jumpx + (target.Y - Vec.Y));
                target.X = (int)(jumpy + (target.X - Vec.X));
            }
        }

        public void Update(Gebieter game)
        {
            temp = game.Regfont;
            rec = new Rectangle((int)(Vec.X + .5f), (int)(Vec.Y + .5f), 8, 8);
            tardistanceX = (Vec.X - target.Center.X);
            tardistanceY = (Vec.Y - target.Center.Y);

            if (rec.Intersects(target) == false)
            {
                missleRotation = (float)Math.Atan2(tardistanceY, tardistanceX);
                Vec.X -= (float)(1.5*Math.Cos(missleRotation));
                Vec.Y -= (float)(1.5*Math.Sign(missleRotation));
            }
            else
            {
                Explode();
            }

            #region exploding
            if (exploding)
            {
                explodeTickCounter++;
                if (explodeTickCounter == 6)
                {
                    explodeTickCounter = 0;
                    if (explodeSourceRect.X + explodeSourceRect.Width >= explodeTexture.Width)
                    {
                        // reached the end of the sequence
                        // not exploding any more
                        exploding = false;
                        exploded = true;
                    }
                    else
                    {
                        // Move on to the next frame
                        explodeSourceRect.X += explodeSourceRect.Width;

                        if (Enemy == "network")
                        {
                            foreach (baseSprite ene in game.Enemies)
                            {
                                if (ene.spriteRectangle.Intersects(explodeRec))
                                {
                                    ene.UpdateHealth(-2);
                                }
                            }
                        }
                        if (Enemy == "humans")
                        {
                            foreach (baseSprite human in game.Humans)
                            {
                                if (human.spriteRectangle.Intersects(explodeRec))
                                {
                                    human.UpdateHealth(-2);
                                }
                            }
                        }
                        if (Enemy == "seed")
                        {
                                game.createAISprite(1, rec.X, rec.Y, rec.X, rec.Y);
                        }
                    }
                }
            }

            #endregion
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (exploding)
            {
                if (Enemy == "network")spritebatch.Draw(explodeTexture, explodeRec, explodeSourceRect, Color.White);
                if (Enemy == "humans") spritebatch.Draw(explodeTexture, explodeRec, explodeSourceRect, Color.Red);
            }
            else
            {
                Rectangle pos = new Rectangle((int)(Vec.X + .5f), (int)(Vec.Y + .5f), sizeX, sizeY);
                spritebatch.Draw(tex, pos, null, Color.White, missleRotation, new Vector2(pos.Width + 30, pos.Height), SpriteEffects.None, 0);
            }
            //spritebatch.DrawString(temp, ""+tardistanceX, new Vector2(rec.Right,rec.Y), Color.Red);
            //spritebatch.DrawString(temp, ""+tardistanceY, new Vector2(rec.X,rec.Bottom), Color.Yellow);
        }

        /// <summary>
        /// runs the death animation of units
        /// </summary>
        public void Explode()
        {
            if (exploding)
            {
                return;
            }
            explodeSound.Play();
            explodeRec = new Rectangle(rec.X - 40, rec.Y - 40, 80, 80);
            explodeSourceRect.X = 0;
            explodeTickCounter = 0;
            exploding = true;
        }
    }
    #endregion 

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Gebieter : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public TempleOrResearch hCommand;
        public Map mAp1;

        #region lists

        //generic list, all inclusive
        public List<baseSprite> Humans = new List<baseSprite>();

        public List<baseSprite> Enemies = new List<baseSprite>();

        //specific lists
        public List<baseSprite> HumanAirList = new List<baseSprite>();
        public List<baseSprite> HumanLandList = new List<baseSprite>();
        public List<baseSprite> HumanBuildingList = new List<baseSprite>();
        public List<baseSprite> buildAreasList = new List<baseSprite>();
        public List<smoke> smokelist = new List<smoke>();
        public List<missiles> miss = new List<missiles>();

        public List<baseSprite> EnemyAirList = new List<baseSprite>();
        public List<baseSprite> EnemyLandList = new List<baseSprite>();
        public List<baseSprite> EnemyBuildingList = new List<baseSprite>();
        public List<NetTentical> tenticalList = new List<NetTentical>();
        #endregion

        #region other state needs
        //title
        bool flarestart;
        Texture2D title;
        Texture2D flare;
        Rectangle flareRec;
        //galactic overview
        Texture2D galacticTex;
        string mapinfo;
        Rectangle Map1;
        bool cleardMap1;
        Rectangle Map2;
        bool cleardMap2;
        Rectangle Map3;
        bool cleardMap3;
        Rectangle Map4;
        bool cleardMap4;
        //game messages
        public SoundEffect resourceError;
        //main song
        bool startedSong;
        Song mainsong;
        Song map1Song;
        Song map2Song;
        Song map3Song;
        Song map4Song;
        Song VictorySong;
        #endregion

        #region mechanics

        public float displayWidth;
        public float displayHieght;

        public KeyboardState keys;
        public KeyboardState oldkeys;
        public MouseState mouse;
        public MouseState oldmouse;
        public GamePadState pad;
        public GamePadState oldpad;
        float padx;
        float pady;
        public Rectangle CursorRec;

        public string direction;

        int smokeletout;

        public bool skipUpdate;

        bool Victory;
        #endregion

        #region state

        enum Gamestate
        {
            title,
            galaticOverView,
            helpscreen,
            gameplay,
            postgame
        }
        Gamestate state = Gamestate.title;
        
        #endregion

        #region Player Mechanics

        #region hud

        string indi;
        int clickcounter;
        bool doubleclick;
        bool anchored;
        //defult cursor
        Texture2D SelectorTex;
        //information display
        Texture2D HudTex;
        public Rectangle HudRec;
        public SpriteFont Regfont;
        Vector2 viewscreen;
        public Rectangle miniMap;
        float jumpdegreeX;
        float jumpdegreeY;
        SpriteFont Smallfont;
        SpriteFont TitleFont;

        int resouceStep = 60;
        public int resouces;
        public Texture2D BlankTex;

        //unit counters
        Texture2D FlyerCounter;
        Texture2D TransportCounter;
        Texture2D TankCounter;
        Texture2D DisCounter;

        int numofFlyers;
        int averageFLyershealth;

        int numofTransports;
        int averageTransportshealth;

        int numofTanks;
        int averageTankshealth;

        int numofDIs;
        int averageDisHealth;
        #endregion

        #region unit controls

        Texture2D pickup;
        bool pickingup;

        bool setstrike;
        Rectangle strikearea;
        public bool clicked;
        public bool selecting;
        public bool cancel;
        //sets wapoint for selected units
        public bool setWaypoint;
        Texture2D waypointTex;
        Rectangle waypointRec;
        public Rectangle AirunitBoxCheck;
        public Rectangle LandUnitBoxCheck;
        public int airrowStep;
        public int airrowYnum;
        public int landrowStep;
        public int landrowYnum;
        int rowCorrectionnumber;
        //icon shown when draging selector box
        Texture2D dragIcon;
        Texture2D NodragIcon;
        //selector box
        Texture2D DragBarTex;
        public Rectangle DragBarRec;

        SoundEffect tankBuilt;
        SoundEffect disBuilt;
        SoundEffect flyerbuilt;
        SoundEffect tranbuilt;

        SoundEffect tankGreet;
        SoundEffect DisGreet1;
        SoundEffect DisGreet2;
        SoundEffect flyerGreet;
        SoundEffect transportGreet;
        #endregion

        #region Building

        bool confirmeDestroyBuilding;
        public bool building;
        Rectangle buildIconRec;
        Rectangle buildingCursorRec;
        Texture2D buildingSelectorTex;

        Rectangle SelectAirFactRec;
        Rectangle SelectLandFactRec;
        Rectangle SelectTurretRec;
        Rectangle SelectAreaEnhanRec;

        int buildnumber;
        bool clearToBuild;
        Color OkBuildIndicatorColor;
        Rectangle productiveBuildingPlacmentRec;
        Rectangle turretPlacementRec;
        Texture2D AirPlacementTex;
        Texture2D LandPlacementTex;
        Texture2D TurretPlacementTex;
        Texture2D AreaEnhanPlacementTex;

        int buildtime;
        int buildcost;
        bool showBuildingbox;
        bool showUnitbox;
        #endregion

        public void hudLoad()
        {
            #region hud
            //hud
            HudTex = Content.Load<Texture2D>("Hud/Combine");
            HudRec = new Rectangle(0, (int)(displayHieght - (displayHieght / 5)), (int)displayWidth, (int)(displayHieght / 5));
            miniMap = new Rectangle((int)(HudRec.Right-(displayWidth/2.9f)),(int)(HudRec.Bottom - (displayWidth/9.7f)),(int)(displayWidth / 10),(int)(displayWidth/10));
            Regfont = Content.Load<SpriteFont>("Fonts/RegHudfont");
            Smallfont = Content.Load<SpriteFont>("Fonts/SmallHudfont");
            TitleFont = Content.Load<SpriteFont>("Fonts/TitleFont");

            BlankTex = Content.Load<Texture2D>("Hud/HealthBar");

            //building
            buildIconRec = new Rectangle((int)(displayWidth / 7.25f), (int)(displayHieght / 1.185f), (int)(displayWidth / 22), (int)(displayWidth / 23));

            buildingCursorRec = new Rectangle((int)(displayWidth / 5.1f), (int)(displayHieght / 1.185f), (int)(displayWidth / 3.15f), (int)(displayHieght / 15));
            buildingSelectorTex = Content.Load<Texture2D>("Hud/BuildTextures/BuildSelector");

            SelectAirFactRec = new Rectangle((int)(displayWidth / 5.1f), (int)(displayHieght / 1.185f), (int)displayWidth / 24, (int)displayWidth / 24);
            SelectLandFactRec = new Rectangle((int)(displayWidth / 3.67f), (int)(displayHieght / 1.185f), (int)displayWidth / 24, (int)displayWidth / 24);
            SelectTurretRec = new Rectangle((int)(displayWidth / 2.85f), (int)(displayHieght / 1.185f), (int)displayWidth / 24, (int)displayWidth / 24);
            SelectAreaEnhanRec = new Rectangle((int)(displayWidth / 2.3f), (int)(displayHieght / 1.185f), (int)displayWidth / 24, (int)displayWidth / 24);

            AirPlacementTex = Content.Load<Texture2D>("Hud/Stencils/AirFactoryStenc");
            LandPlacementTex = Content.Load<Texture2D>("Hud/Stencils/HumanLandFactoryStenc");
            TurretPlacementTex = Content.Load<Texture2D>("Hud/Stencils/TurretStenc");
            AreaEnhanPlacementTex = Content.Load<Texture2D>("Hud/Stencils/AreaEnhancerStenc");

            productiveBuildingPlacmentRec = new Rectangle(0, 0, (int)(displayWidth * .13f), (int)(displayWidth * .13f));
            turretPlacementRec = new Rectangle(0, 0, (int)(displayWidth * .1f), (int)(displayWidth * .1f));

            //counting
            FlyerCounter = Content.Load<Texture2D>("Hud/BuildTextures/BuildFlyerButton");
            TransportCounter = Content.Load<Texture2D>("Hud/BuildTextures/BuildTransportButton");
            TankCounter = Content.Load<Texture2D>("Hud/BuildTextures/BuildTankButton");
            DisCounter = Content.Load<Texture2D>("Hud/BuildTextures/BuildDISButton");
            #endregion

            #region selector
            //selector
            SelectorTex = Content.Load<Texture2D>("Hud/Selector/Selector");
            CursorRec = new Rectangle(0, 0, (int)displayWidth / 40, (int)displayWidth / 40);
            pickup = Content.Load<Texture2D>("Humans/Units/Pickup");

            //waypoint
            waypointTex = Content.Load<Texture2D>("Hud/Selector/Waypoint");
            waypointRec = new Rectangle(-100, -100, 1, 1);

            //dragging
            DragBarTex = Content.Load<Texture2D>("Hud/Selector/DragBar");
            dragIcon = Content.Load<Texture2D>("Hud/Selector/DragIcon");
            NodragIcon = Content.Load<Texture2D>("Hud/Selector/NODragIcon");
            #endregion

            #region unit greets
            disBuilt = Content.Load<SoundEffect>("Humans/Greets/DisBuilt");
            tankBuilt = Content.Load<SoundEffect>("Humans/Greets/tankBuilt");
            flyerbuilt = Content.Load<SoundEffect>("Humans/Greets/FlyerBuilt");
            tranbuilt = Content.Load<SoundEffect>("Humans/Greets/tranBuilt");

            tankGreet = Content.Load<SoundEffect>("Humans/Greets/TankGreet");
            DisGreet1 = Content.Load<SoundEffect>("Humans/Greets/DisAttack");
            DisGreet2 = Content.Load<SoundEffect>("Humans/Greets/Dismove");
            flyerGreet = Content.Load<SoundEffect>("Humans/Greets/FlyerGreet");
            transportGreet = Content.Load<SoundEffect>("Humans/Greets/TranGreet");
            #endregion
        }

        public void hudUpdate()
        {
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Q) && oldkeys.IsKeyUp(Keys.Q)) createHumanSprite(7, mouse.X, mouse.Y, mouse.X, mouse.Y);
            if (keys.IsKeyDown(Keys.W) && oldkeys.IsKeyUp(Keys.W)) createHumanSprite(1, mouse.X, mouse.Y, mouse.X, mouse.Y);
            if (keys.IsKeyDown(Keys.E) && oldkeys.IsKeyUp(Keys.E)) createHumanSprite(2, mouse.X, mouse.Y, mouse.X, mouse.Y);
            //ends game if command is destroyed
            if (hCommand.health <= 0)
            {
                Victory = false;
                resetSongs();
                state = Gamestate.postgame;
            }

            #region selects units / cancel selection

            if (mouse.LeftButton == ButtonState.Pressed || pad.Buttons.A == ButtonState.Pressed)
            {
                if (anchored == false)
                {
                    DragBarRec.X = CursorRec.X;
                    DragBarRec.Y = CursorRec.Y;
                    anchored = true;
                }
                if (CursorRec.X > DragBarRec.X && CursorRec.Y > DragBarRec.Y)
                {
                    //moves selecting box
                    DragBarRec.Width = CursorRec.X - DragBarRec.X;
                    DragBarRec.Height = CursorRec.Y - DragBarRec.Y;
                    selecting = true;
                }
                else selecting = false;
            }
            else
            {
                anchored = false;
                //resets box
                DragBarRec.X = -10;
                DragBarRec.Y = -10;
                //moves icon
                selecting = false;
            }
            //cancels selection
            if (keys.IsKeyDown(Keys.Delete) || pad.Buttons.X == ButtonState.Pressed)
            {
                cancel = true;
            }
            else cancel = false;

            #endregion

            if (paused == false)
            {
                #region resources
                //regulates rescouces
                if (resouceStep > 0) resouceStep--;
                if (resouceStep == 0)
                {
                    resouceStep = 60;
                    if (resouces < 9995) resouces += (2 * buildAreasList.Count);
                }
                #endregion

                #region unit counter
                averageFLyershealth = 0;
                numofFlyers = 0;
                averageTransportshealth = 0;
                numofTransports = 0;

                averageTankshealth = 0;
                numofTanks = 0;
                averageDisHealth = 0;
                numofDIs = 0;

                foreach (BaseUnit airunit in HumanAirList)
                {
                    if (airunit.state == BaseUnit.BaseUnitState.selected)
                    {
                        if (airunit.indicator == "flyer") { numofFlyers++; averageFLyershealth += (int)airunit.health; }

                        if (airunit.indicator == "tran") { numofTransports++; averageTransportshealth += (int)airunit.health; }
                    }
                }
                foreach (BaseUnit landunit in HumanLandList)
                {
                    if (landunit.state == BaseUnit.BaseUnitState.selected && landunit.inTransport == false)
                    {
                        if (landunit.indicator == "tank") { numofTanks++; averageTankshealth += (int)landunit.health; }
                        if (landunit.indicator == "Disruptor") { numofDIs++; averageDisHealth += (int)landunit.health; }
                    }
                }
                #endregion

                #region areaStrike

                if (keys.IsKeyDown(Keys.NumPad0) || pad.Buttons.B == ButtonState.Pressed)
                {
                    setstrike = true;
                }
                else setstrike = false;

                #endregion

                #region waypoint assigning

                //checks if the player clicked on a enemy to be attacked
                if (mouse.RightButton == ButtonState.Pressed && oldmouse.RightButton == ButtonState.Released || pad.Buttons.Y == ButtonState.Pressed && oldpad.Buttons.Y == ButtonState.Released)
                {
                    //waypoint animation setup
                    waypointRec.X = (CursorRec.X - 30);
                    waypointRec.Y = (CursorRec.Y - 30);
                    waypointRec.Width = (int)(displayWidth / 10);
                    waypointRec.Height = waypointRec.Width;

                    //sets waypoint for selected units
                    if (setstrike == false)
                    {
                        //sets rows / columns
                        int numofselcetedland = 0;
                        int numofselcetedAir = 0;
                        airrowStep = 0;
                        airrowYnum = 0;
                        landrowStep = 0;
                        landrowYnum = 0;

                        #region Land unit box

                        foreach (BaseUnit Landunit in HumanLandList)
                        {
                            if (Landunit.state == BaseUnit.BaseUnitState.selected)
                            {
                                numofselcetedland++;
                            }
                        }
                        foreach (BaseUnit Landunit in HumanLandList)
                        {
                            if (Landunit.state == BaseUnit.BaseUnitState.selected)
                            {
                                #region sets row number
                                //sets rows to 5
                                if (numofselcetedland <= 25)
                                {
                                    if (landrowStep >= 5)
                                    {
                                        landrowStep = 0;
                                        landrowYnum++;
                                    }
                                    rowCorrectionnumber = 5;
                                }
                                //sets rows to 8
                                if (numofselcetedland > 25 && numofselcetedland <= 64)
                                {
                                    if (landrowStep >= 8)
                                    {
                                        landrowStep = 0;
                                        landrowYnum++;
                                    }
                                    rowCorrectionnumber = 8;
                                }
                                //sets rows to 10
                                if (numofselcetedland > 64)
                                {
                                    if (landrowStep >= 10)
                                    {
                                        landrowStep = 0;
                                        landrowYnum++;
                                    }
                                    rowCorrectionnumber = 10;
                                }
                                #endregion
                                // magic stuff that makes boxes along with the waypointassigning method (below)
                                LandUnitBoxCheck = waypointassigningCalculations(Landunit.spriteRectangle.Width, Landunit.spriteRectangle.Height, numofselcetedland, rowCorrectionnumber);

                                Landunit.waypoint.X = (int)(LandUnitBoxCheck.X + (landrowStep * ((Landunit.spriteRectangle.Width) * 1.2f)));
                                Landunit.waypoint.Y = (int)(LandUnitBoxCheck.Y + (landrowYnum * ((Landunit.spriteRectangle.Height) * 1.2f)));

                                landrowStep++;
                                Landunit.setStrike = false;
                            }
                        }
                        #endregion
                        #region air unit box
                        foreach (BaseUnit Airunit in HumanAirList)
                        {
                            if (Airunit.state == BaseUnit.BaseUnitState.selected)
                            {
                                numofselcetedAir++;
                            }
                        }
                        foreach (BaseUnit Airunit in HumanAirList)
                        {
                            #region sets rows
                            //sets rows to 5
                            if (numofselcetedAir <= 25)
                            {
                                if (airrowStep >= 5)
                                {
                                    airrowStep = 0;
                                    airrowYnum++;
                                }
                                rowCorrectionnumber = 5;
                            }
                            //sets rows to 8
                            if (numofselcetedAir > 25 && numofselcetedAir <= 64)
                            {
                                if (airrowStep >= 8)
                                {
                                    airrowStep = 0;
                                    airrowYnum++;
                                }
                                rowCorrectionnumber = 8;
                            }
                            //sets rows to 10
                            if (numofselcetedAir > 64)
                            {
                                if (airrowStep >= 10)
                                {
                                    airrowStep = 0;
                                    airrowYnum++;
                                }
                                rowCorrectionnumber = 10;
                            }
                            #endregion
                            if (Airunit.state == BaseUnit.BaseUnitState.selected)
                            {
                                // magic stuff that makes boxes along with the waypointassigning method (below)
                                AirunitBoxCheck = waypointassigningCalculations(Airunit.spriteRectangle.Width, Airunit.spriteRectangle.Height, numofselcetedAir, rowCorrectionnumber);

                                Airunit.waypoint.X = (int)(AirunitBoxCheck.X + (airrowStep * (Airunit.spriteRectangle.Width * 1.5f)));
                                Airunit.waypoint.Y = (int)(AirunitBoxCheck.Y + (airrowYnum * (Airunit.spriteRectangle.Height * 1.5)));

                                airrowStep++;
                                Airunit.setStrike = false;
                            }
                        }
                        #endregion
                        setWaypoint = true;
                    }
                    else
                    {
                        strikearea = new Rectangle(CursorRec.X - (int)(displayWidth / 8), CursorRec.Y - (int)(displayWidth / 8), (int)(displayWidth / 4), (int)(displayWidth / 4));
                        foreach (BaseUnit air in HumanAirList)
                        {
                            if (air.state == BaseUnit.BaseUnitState.selected)
                            {
                                air.setStrike = true;
                                air.strikearea = strikearea;
                                air.waypoint.X = strikearea.Center.X;
                                air.waypoint.Y = strikearea.Center.Y;
                                air.state = BaseUnit.BaseUnitState.moving;
                            }
                        }
                        foreach (BaseUnit land in HumanLandList)
                        {
                            if (land.state == BaseUnit.BaseUnitState.selected)
                            {
                                land.setStrike = true;
                                land.strikearea = strikearea;
                                land.waypoint.X = strikearea.Center.X;
                                land.waypoint.Y = strikearea.Center.Y;
                                land.state = BaseUnit.BaseUnitState.moving;
                            }
                        }
                    }
                    if (numofDIs > 0)
                    {
                        if (setstrike) DisGreet1.Play();
                        else  DisGreet2.Play();
                    }
                    else
                    {
                        if (numofFlyers > 0) flyerGreet.Play();
                        else
                        {
                            if (numofTransports > 0) transportGreet.Play();
                            else
                            {
                                if (numofTanks > 0) tankGreet.Play();
                            }
                        }
                    }

                }
                else setWaypoint = false;

                #endregion

                #region waypointflash

                if (waypointRec.Width > (int)(displayWidth / 40))
                {
                    waypointRec.Width -= 4;
                    waypointRec.X += 2;
                    waypointRec.Height -= 4;
                    waypointRec.Y += 2;
                }

                #endregion

                #region building

                if (CursorRec.Intersects(buildIconRec) && mouse.LeftButton == ButtonState.Pressed || CursorRec.Intersects(buildIconRec) && pad.Buttons.A == ButtonState.Pressed) { building = true; }
                if (cancel) { building = false; buildnumber = 0; }
                if (building == true)
                {
                    if (buildnumber == 0)
                    {
                        if (CursorRec.Intersects(SelectAirFactRec) && CursorRec.X > SelectAirFactRec.X && clicked == true)
                        {
                            if (resouces >= 200)
                            {
                                buildnumber = 3;
                            }
                            else resourceError.Play();
                        }
                        if (CursorRec.Intersects(SelectLandFactRec) && CursorRec.X > SelectLandFactRec.X && clicked == true)
                        {
                            if (resouces >= 200)
                            {
                                buildnumber = 4;
                            }
                            else resourceError.Play();
                        }
                        if (CursorRec.Intersects(SelectTurretRec) && CursorRec.X > SelectTurretRec.X && clicked == true)
                        {
                            if (resouces >= 150)
                            {
                                buildnumber = 5;
                            }
                            else resourceError.Play();
                        }
                        if (CursorRec.Intersects(SelectAreaEnhanRec) && CursorRec.X > SelectAreaEnhanRec.X && clicked == true)
                        {
                            if (resouces >= 100)
                            {
                                buildnumber = 6;
                            }
                            else resourceError.Play();
                        }

                        if (CursorRec.Intersects(SelectAirFactRec) == false && CursorRec.Intersects(SelectLandFactRec) == false
                            && CursorRec.Intersects(SelectTurretRec) == false && CursorRec.Intersects(SelectAreaEnhanRec) == false && clicked == true)
                        {
                            buildnumber = 0;
                            building = false;
                        }
                    }
                    else
                    {
                        if (buildnumber == 3 || buildnumber == 4) { productiveBuildingPlacmentRec.X = CursorRec.X; productiveBuildingPlacmentRec.Y = CursorRec.Y; }
                        if (buildnumber == 5 || buildnumber == 6) { turretPlacementRec.X = CursorRec.X; turretPlacementRec.Y = CursorRec.Y; }

                        //this checks if the building is in a building area, if so it gives the ok and breaks
                        foreach (AreaEnhancer buildarea in buildAreasList)
                        {
                            if (buildnumber == 3 || buildnumber == 4)
                            {
                                if (productiveBuildingPlacmentRec.Intersects(buildarea.bluearea))
                                {
                                    clearToBuild = true;
                                    break;
                                }
                                else clearToBuild = false;
                            }
                            if (buildnumber == 5 || buildnumber == 6)
                            {
                                if (turretPlacementRec.Intersects(buildarea.bluearea))
                                {
                                    clearToBuild = true;
                                    break;
                                }
                                else clearToBuild = false;
                            }
                        }

                        foreach (baseSprite structure in HumanBuildingList)
                        {
                            if (buildnumber == 3 || buildnumber == 4)
                            {
                                if (productiveBuildingPlacmentRec.Intersects(structure.spriteRectangle))
                                {
                                    clearToBuild = false;
                                    break;
                                }
                                if (productiveBuildingPlacmentRec.X < (mAp1.spriteRectangle.X + (displayWidth / 10)) || productiveBuildingPlacmentRec.Right > (mAp1.spriteRectangle.Right - (displayWidth / 10))
                                    || productiveBuildingPlacmentRec.Y < (mAp1.spriteRectangle.Y + (displayWidth / 10)) || productiveBuildingPlacmentRec.Bottom > (mAp1.spriteRectangle.Bottom - (displayWidth / 10)))
                                {
                                    clearToBuild = false;
                                    break;
                                }
                            }
                            if (buildnumber == 5 || buildnumber == 6)
                            {
                                if (turretPlacementRec.Intersects(structure.spriteRectangle))
                                {
                                    clearToBuild = false;
                                    break;
                                }
                                if (turretPlacementRec.X < (mAp1.spriteRectangle.X + (displayWidth / 10)) || turretPlacementRec.Right > (mAp1.spriteRectangle.Right - (displayWidth / 10))
                                    || turretPlacementRec.Y < (mAp1.spriteRectangle.Y + (displayWidth / 10)) || turretPlacementRec.Bottom > (mAp1.spriteRectangle.Bottom - (displayWidth / 10)))
                                {
                                    clearToBuild = false;
                                    break;
                                }
                            }
                            //if here, the building isnt intersecting any building and is in a build area
                        }

                        if (clearToBuild == true && clicked)
                        {
                            if (buildnumber == 3 || buildnumber == 4)
                            {
                                resouces -= 200;
                                createHumanSprite(buildnumber, (int)(CursorRec.X + ((displayWidth * .04f) / 2)), (int)(CursorRec.Y + ((displayWidth * .04f) / 2)), 0, 0);
                                buildnumber = 0;
                            }
                            if (buildnumber == 5)
                            {
                                resouces -= 150;
                                createHumanSprite(buildnumber, (int)(CursorRec.X + ((displayWidth * .05f) / 2)), (int)(CursorRec.Y + ((displayWidth * .05f) / 2)), 0, 0);
                                buildnumber = 0;
                            }
                            if (buildnumber == 6)
                            {
                                resouces -= 100;
                                createHumanSprite(buildnumber, (int)(CursorRec.X + ((displayWidth * .05f) / 2)), (int)(CursorRec.Y + ((displayWidth * .05f) / 2)), 0, 0);
                                buildnumber = 0;
                            }
                        }
                    }
                    //sets clear to build indication color
                    if (clearToBuild == true) { OkBuildIndicatorColor = new Color(100, 200, 100, 200); }
                    else { OkBuildIndicatorColor = new Color(200, 100, 100, 200); }
                }
                #endregion

                #region mass selecting

                if (doubleclick == true && CursorRec.Intersects(HudRec) == false)
                {
                    foreach (baseSprite sprite in Humans)
                    {
                        if (CursorRec.Intersects(sprite.spriteRectangle) && sprite.indicator != "temple") indi = sprite.indicator;
                    }
                    //checks for buildings
                    if (indi == "airfact" || indi == "landfact" || indi == "turret" || indi == "areaEnhancer")
                    {
                        foreach (HumanBuilding buil in HumanBuildingList)
                        {
                            if (buil.spriteRectangle.X > 0 && buil.spriteRectangle.Y > 0
                                && buil.spriteRectangle.Right < displayWidth && buil.spriteRectangle.Bottom < displayHieght
                                && buil.indicator == indi)
                            {
                                buil.state = HumanBuilding.BaseBuildingState.selected;
                            }
                        }
                    }
                    //checks for air
                    if (indi == "flyer" || indi == "tran")
                    {
                        foreach (BaseUnit unit in HumanAirList)
                        {
                            if (unit.spriteRectangle.X > 0 && unit.spriteRectangle.Y > 0
                                && unit.spriteRectangle.Right < displayWidth && unit.spriteRectangle.Bottom < displayHieght
                                && unit.indicator == indi)
                            {
                                unit.state = BaseUnit.BaseUnitState.selected;
                            }
                        }
                    }
                    //checks for land
                    if (indi == "tank" || indi == "Disruptor")
                    {
                        foreach (BaseUnit unit in HumanLandList)
                        {
                            if (unit.spriteRectangle.X > 0 && unit.spriteRectangle.Y > 0
                                && unit.spriteRectangle.Right < displayWidth && unit.spriteRectangle.Bottom < displayHieght
                                && unit.indicator == indi)
                            {
                                unit.state = BaseUnit.BaseUnitState.selected;
                            }
                        }
                    }
                    indi = "";
                }

                #endregion

                #region tranpickup switch

                foreach (BaseUnit tran in HumanAirList)
                {
                    if (numofTanks > 0 && CursorRec.Intersects(tran.spriteRectangle) && tran.indicator == "tran"||
                        numofDIs > 0 && CursorRec.Intersects(tran.spriteRectangle) && tran.indicator == "tran") pickingup = true;
                    else pickingup = false;
                }

                #endregion

                #region carbage man

                if (keys.IsKeyDown(Keys.Space) && oldkeys.IsKeyUp(Keys.Space) || pad.Buttons.BigButton == ButtonState.Pressed && oldpad.Buttons.BigButton == ButtonState.Released)
                {
                    foreach (HumanBuilding build in HumanBuildingList)
                    {
                        if (build.state == HumanBuilding.BaseBuildingState.selected && build.indicator != "temple")
                        {
                            confirmeDestroyBuilding = true;
                            break;
                        }
                    }
                }
                if (confirmeDestroyBuilding == true)
                {
                    if (keys.IsKeyDown(Keys.Y) || pad.Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        foreach (HumanBuilding build in HumanBuildingList)
                        {
                            if (build.state == HumanBuilding.BaseBuildingState.selected)
                            {
                                build.Explode();
                            }
                        }
                        confirmeDestroyBuilding = false;
                    }
                    if (keys.IsKeyDown(Keys.N) || pad.Buttons.RightShoulder == ButtonState.Pressed || clicked) confirmeDestroyBuilding = false;
                }

                ////removes dead sprites
                foreach (baseSprite sprite in HumanAirList)
                {
                    if (sprite.dead)
                    {
                        HumanAirList.Remove(sprite);
                        Humans.Remove(sprite);
                        break;
                    }
                }
                foreach (baseSprite sprite in HumanLandList)
                {
                    if (sprite.dead)
                    {
                        HumanLandList.Remove(sprite);
                        Humans.Remove(sprite);
                        break;
                    }
                }
                foreach (baseSprite sprite in HumanBuildingList)
                {
                    if (sprite.dead)
                    {
                        HumanBuildingList.Remove(sprite);
                        Humans.Remove(sprite);
                        break;
                    }
                }
                foreach (smoke smok in smokelist)
                {
                    if (smok.dispersed)
                    {
                        smokelist.Remove(smok);
                        break;
                    }
                }
                foreach (missiles mis in miss)
                {
                    if (mis.exploded)
                    {
                        miss.Remove(mis);
                        break;
                    }
                }
                #endregion

                #region mapjump

                if (clicked && CursorRec.Intersects(miniMap) && CursorRec.X > miniMap.X && CursorRec.Y > miniMap.Y)
                {
                    skipUpdate = true; 

                    jumpdegreeX = (CursorRec.X - miniMap.X) * -30;
                    jumpdegreeY = (CursorRec.Y - miniMap.Y) * -30;

                    float oldmapX = mAp1.spriteRectangle.X;
                    float oldmapY = mAp1.spriteRectangle.Y;

                    mAp1.move("", true, jumpdegreeX, jumpdegreeY);
                    foreach (baseSprite sprite in Humans)
                    {
                        float newlocatX = ((sprite.spriteRectangle.X - oldmapX) + jumpdegreeX);
                        float newlocatY = ((sprite.spriteRectangle.Y - oldmapY) + jumpdegreeY);
                        sprite.move("", true, newlocatX, newlocatY);
                    }
                    foreach (baseSprite sprite in Enemies)
                    {
                        float newlocatX = ((sprite.spriteRectangle.X - oldmapX) + jumpdegreeX);
                        float newlocatY = ((sprite.spriteRectangle.Y - oldmapY) + jumpdegreeY);
                        sprite.move("", true, newlocatX, newlocatY);
                    }
                    foreach (NetTentical tent in tenticalList)
                    {
                        float newlocatX = ((tent.spriteRec.X - oldmapX) + jumpdegreeX);
                        float newlocatY = ((tent.spriteRec.Y - oldmapY) + jumpdegreeY);
                        tent.move("", true, newlocatX, newlocatY);
                    }
                    foreach (smoke smo in smokelist)
                    {
                        float newlocatX = ((smo.smokeRec.X - oldmapX) + jumpdegreeX);
                        float newlocatY = ((smo.smokeRec.Y - oldmapY) + jumpdegreeY);
                        smo.move("", true, newlocatX, newlocatY);
                    }
                    foreach (missiles mis in miss)
                    {
                        float newlocatX = ((mis.rec.X - oldmapX) + jumpdegreeX);
                        float newlocatY = ((mis.rec.Y - oldmapY) + jumpdegreeY);
                        mis.move("", true, newlocatX, newlocatY);
                    }
                    viewscreen.X = (CursorRec.X - 5);
                    viewscreen.Y = (CursorRec.Y - 7);
                }
                #endregion

                #region build info
                if (building && buildnumber == 0)
                {
                    if (CursorRec.Intersects(SelectAirFactRec) && CursorRec.X > SelectAirFactRec.X && CursorRec.Y > SelectAirFactRec.Y ||
                        CursorRec.Intersects(SelectLandFactRec) && CursorRec.X > SelectLandFactRec.X && CursorRec.Y > SelectLandFactRec.Y ||
                        CursorRec.Intersects(SelectTurretRec) && CursorRec.X > SelectTurretRec.X && CursorRec.Y > SelectTurretRec.Y ||
                        CursorRec.Intersects(SelectAreaEnhanRec) && CursorRec.X > SelectAreaEnhanRec.X && CursorRec.Y > SelectAreaEnhanRec.Y)
                    {
                        if (CursorRec.Intersects(SelectAirFactRec) && CursorRec.X > SelectAirFactRec.X && CursorRec.Y > SelectAirFactRec.Y ||
                            CursorRec.Intersects(SelectLandFactRec) && CursorRec.X > SelectLandFactRec.X && CursorRec.Y > SelectLandFactRec.Y)
                        {
                            buildtime = 10;
                            buildcost = 200;
                            showBuildingbox = true;
                        }
                        if (CursorRec.Intersects(SelectTurretRec) && CursorRec.X > SelectTurretRec.X && CursorRec.Y > SelectTurretRec.Y)
                        {
                            buildtime = 6;
                            buildcost = 150;
                            showBuildingbox = true;
                        }
                        if (CursorRec.Intersects(SelectAreaEnhanRec) && CursorRec.X > SelectAreaEnhanRec.X && CursorRec.Y > SelectAreaEnhanRec.Y)
                        {
                            buildtime = 6;
                            buildcost = 100;
                            showBuildingbox = true;
                        }
                    }
                    else { showBuildingbox = false;}
                }
                else { showBuildingbox = false;}
                foreach (HumanBuilding build in HumanBuildingList)
                {
                    if (build.state == HumanBuilding.BaseBuildingState.selected)
                    {
                        if (CursorRec.Intersects(build.UnitButton1Rec) && CursorRec.X > build.UnitButton1Rec.X && CursorRec.Y > build.UnitButton1Rec.Y ||
                            CursorRec.Intersects(build.UnitButton2Rec) && CursorRec.X > build.UnitButton2Rec.X && CursorRec.Y > build.UnitButton2Rec.Y)
                        {
                            if (CursorRec.Intersects(build.UnitButton1Rec) && CursorRec.X > build.UnitButton1Rec.X && CursorRec.Y > build.UnitButton1Rec.Y)
                            {
                                buildtime = build.unit1BuildTime/60;
                                buildcost = build.unit1buildcost;
                                showUnitbox = true;
                            }
                            if (CursorRec.Intersects(build.UnitButton2Rec) && CursorRec.X > build.UnitButton2Rec.X && CursorRec.Y > build.UnitButton2Rec.Y)
                            {
                                buildtime = build.unit2BuildTime/60;
                                buildcost = build.unit2buildcost;
                                showUnitbox = true;
                            }
                        }
                        else { showUnitbox = false; }
                    }
                }

            #endregion
            }
        }

        public void hudDraw()
        {
            #region hud
            spriteBatch.Draw(HudTex, HudRec, Color.White);
            spriteBatch.DrawString(Regfont, "" + (HumanAirList.Count + HumanLandList.Count), new Vector2(displayWidth / 3.3f, displayHieght / 1.075f), Color.Black);
            spriteBatch.DrawString(Regfont, "" + (HumanBuildingList.Count - 1), new Vector2(displayWidth / 2.25f, displayHieght / 1.075f), Color.Black);
            spriteBatch.DrawString(Regfont, "" + resouces, new Vector2(displayWidth / 5.2f, displayHieght / 1.075f), Color.Black);
            //shows infection stats
            spriteBatch.DrawString(Regfont, "____I.A:    ____", new Vector2(displayWidth/1.29f,displayHieght/1.2f), Color.Red);
            if (PercentInfection <= 25) spriteBatch.DrawString(Regfont, PercentInfection + "%", new Vector2(displayWidth / 1.12f, displayHieght / 1.2f), Color.Green);
            if (PercentInfection <= 75 && PercentInfection > 25) spriteBatch.DrawString(Regfont, PercentInfection + "%", new Vector2(displayWidth / 1.12f, displayHieght / 1.2f), Color.Yellow);
            if (PercentInfection > 75) spriteBatch.DrawString(Regfont, PercentInfection + "%", new Vector2(displayWidth / 1.12f, displayHieght / 1.2f), Color.Red);
            spriteBatch.DrawString(Regfont, "N.A.T:" + (infectedTime/60), new Vector2(displayWidth / 1.21f, displayHieght / 1.12f), Color.Red);
            spriteBatch.DrawString(Regfont, "N.H:+" + infectionHealthBoost, new Vector2(displayWidth / 1.29f, displayHieght / 1.06f), Color.Red);
            spriteBatch.DrawString(Regfont, "N.S:+" + infectionDamageBoost, new Vector2(displayWidth / 1.12f, displayHieght / 1.06f), Color.Red);

            #region miniMap
            spriteBatch.Draw(mAp1.spriteTexture, miniMap, Color.White);
            //humans
            foreach (baseSprite sprite in Humans)
            {
                if (sprite.inTransport == false)
                {
                    float locx = miniMap.X + ((Math.Abs(sprite.spriteRectangle.X - mAp1.spriteRectangle.X)) / 30);
                    float locy = miniMap.Y + ((Math.Abs(sprite.spriteRectangle.Y - mAp1.spriteRectangle.Y)) / 30);
                    Rectangle location = new Rectangle((int)locx, (int)locy, 0, 0);
                    if (sprite.indicator == "airfact" || sprite.indicator == "landfact" || sprite.indicator == "turret" || sprite.indicator == "areaEnhancer" || sprite.indicator == "temple")
                    {
                        location = new Rectangle((int)locx, (int)locy, 4, 4);
                    }
                    else
                    {
                        location = new Rectangle((int)locx, (int)locy, 3, 3);
                    }
                    spriteBatch.Draw(BlankTex, location, Color.Green);
                }
            }
            //creative
            foreach (smoke smo in smokelist)
            {
                float locx = miniMap.X + ((Math.Abs(smo.smokeRec.X - mAp1.spriteRectangle.X)) / 30);
                float locy = miniMap.Y + ((Math.Abs(smo.smokeRec.Y - mAp1.spriteRectangle.Y)) / 30);
                Rectangle location = new Rectangle((int)locx, (int)locy, 2, 2);
                spriteBatch.Draw(BlankTex, location, Color.Gray);
            }
            foreach (missiles mis in miss)
            {
                float locx = miniMap.X + ((Math.Abs(mis.rec.X - mAp1.spriteRectangle.X)) / 30);
                float locy = miniMap.Y + ((Math.Abs(mis.rec.Y - mAp1.spriteRectangle.Y)) / 30);
                Rectangle location = new Rectangle((int)locx, (int)locy, 2, 2);
                spriteBatch.Draw(BlankTex, location, Color.Blue);
            }
            //network
            foreach (baseSprite sprite in Enemies)
            {
                float locx = miniMap.X + ((Math.Abs(sprite.spriteRectangle.X - mAp1.spriteRectangle.X)) / 30);
                float locy = miniMap.Y + ((Math.Abs(sprite.spriteRectangle.Y - mAp1.spriteRectangle.Y)) / 30);
                Rectangle location = new Rectangle((int)locx, (int)locy, 0, 0);

                if (sprite.indicator == "aispawner" || sprite.indicator == "aiseed" || sprite.indicator == "aiturret" || sprite.indicator == "network")
                {
                    location = new Rectangle((int)locx, (int)locy, 4, 4);
                }
                else
                {
                    location = new Rectangle((int)locx, (int)locy, 3, 3);
                }
                spriteBatch.Draw(BlankTex, location, Color.Red);
            }

            spriteBatch.DrawString(Regfont, "[ ]", viewscreen, Color.Black);
            #endregion

            if (hCommand.health > 40)
                spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 36.2), (int)(displayHieght / 1.03f), (int)(hCommand.health / 2), (int)(displayHieght / 50)), new Color(34, 177, 76));
            if (hCommand.health <= 40 && hCommand.health > 20)
                spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 36.2), (int)(displayHieght / 1.03f), (int)(hCommand.health / 2), (int)(displayHieght / 50)), Color.Yellow);
            if (hCommand.health <= 20)
                spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 36.2), (int)(displayHieght / 1.03f), (int)(hCommand.health / 2), (int)(displayHieght / 50)), Color.Red);

            #endregion

            #region building

            if (building == true)
            {
                spriteBatch.Draw(buildingSelectorTex, buildingCursorRec, Color.White);
                //spriteBatch.Draw(BlankTex, SelectAirFactRec, Color.White);
                //spriteBatch.Draw(BlankTex, SelectLandFactRec, Color.White);
                //spriteBatch.Draw(BlankTex, SelectTurretRec, Color.White);
                //spriteBatch.Draw(BlankTex, SelectAreaEnhanRec, Color.White);
                if (buildnumber == 3) { spriteBatch.Draw(AirPlacementTex, productiveBuildingPlacmentRec, OkBuildIndicatorColor); }
                if (buildnumber == 4) { spriteBatch.Draw(LandPlacementTex, productiveBuildingPlacmentRec, OkBuildIndicatorColor); }

                if (buildnumber == 5) { spriteBatch.Draw(TurretPlacementTex, turretPlacementRec, OkBuildIndicatorColor); }
                if (buildnumber == 6) { spriteBatch.Draw(AreaEnhanPlacementTex, turretPlacementRec, OkBuildIndicatorColor); }
            }

            if (confirmeDestroyBuilding == true)
            {
                spriteBatch.Draw(BlankTex, new Rectangle((int)displayWidth / 4, 0, (int)displayWidth / 2, (int)displayHieght / 20), Color.Gray);
                spriteBatch.DrawString(Regfont, " Confirm Desruction:Y/N | LB/RB", new Vector2(displayWidth / 3.5f, 0), Color.Red);
            }

            #endregion

            #region cards

            #region buildings
            foreach (HumanBuilding objectofSelecting in HumanBuildingList)
            {
                if (objectofSelecting.state == HumanBuilding.BaseBuildingState.selected)
                {
                    //health
                    spriteBatch.DrawString(Smallfont, "_Health_", new Vector2(displayWidth / 1.86f, displayHieght / 1.2f), Color.Black);
                    spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.83f), (int)(displayHieght / 1.15f), (int)(objectofSelecting.health / 2), (int)(displayHieght / 50)), new Color(34, 177, 76));
                    //Unit one button
                    if (objectofSelecting.UnitButton1Tex != null)
                    {
                        //button
                        spriteBatch.Draw(objectofSelecting.UnitButton1Tex, objectofSelecting.UnitButton1Rec, Color.White);

                        //unit build idicator
                        spriteBatch.DrawString(Smallfont, "" + objectofSelecting.Units1ToBuild, new Vector2(displayWidth / 1.905f, displayHieght / 1.065f), Color.Black);

                        if (objectofSelecting.Units1ToBuild > 0)
                            spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.86f), (int)(displayHieght / 1.05f), objectofSelecting.unit1BuildTime / 20, (int)(displayHieght / 70)), Color.White);
                    }
                    //unit two button
                    if (objectofSelecting.UnitButton2Tex != null)
                    {
                        //button
                        spriteBatch.Draw(objectofSelecting.UnitButton2Tex, objectofSelecting.UnitButton2Rec, Color.White);

                        //unit build idicator
                        spriteBatch.DrawString(Smallfont, "" + objectofSelecting.Units2ToBuild, new Vector2(displayWidth / 1.72f, displayHieght / 1.065f), Color.Black);

                        if (objectofSelecting.Units2ToBuild > 0)
                            spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.68f), (int)(displayHieght / 1.05f), objectofSelecting.unit2BuildTime / 25, (int)(displayHieght / 70)), Color.White);
                    }
                }
            }
            #endregion

            #region units

                if (numofFlyers > 0)
                {
                    spriteBatch.Draw(FlyerCounter, new Rectangle((int)(displayWidth / 1.91f), (int)(displayHieght / 1.19f), (int)(displayWidth / 18), (int)(displayHieght / 13)), Color.White);
                    spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.77f), (int)(displayHieght / 1.18f), (int)(displayWidth / 150), (int)((averageFLyershealth / 4) / numofFlyers)), Color.Green);
                    spriteBatch.DrawString(Smallfont, "" + numofFlyers, new Vector2(displayWidth / 1.84f, displayHieght / 1.15f), Color.Black);
                }
                if (numofTransports > 0)
                {
                    spriteBatch.Draw(TransportCounter, new Rectangle((int)(displayWidth / 1.91f), (int)(displayHieght / 1.097f), (int)(displayWidth / 18), (int)(displayHieght / 13)), Color.White);
                    spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.77f), (int)(displayHieght / 1.09f), (int)(displayWidth / 150), (int)((averageTransportshealth / 4) / numofTransports)), Color.Green);
                    spriteBatch.DrawString(Smallfont, "" + numofTransports, new Vector2(displayWidth / 1.84f, displayHieght / 1.06f), Color.Black);
                }
                if (numofTanks > 0)
                {
                    spriteBatch.Draw(TankCounter, new Rectangle((int)(displayWidth / 1.73f), (int)(displayHieght / 1.19f), (int)(displayWidth / 18), (int)(displayHieght / 13)), Color.White);
                    spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.61f), (int)(displayHieght / 1.18f), (int)(displayWidth / 150), (int)((averageTankshealth / 4) / numofTanks)), Color.Green);
                    spriteBatch.DrawString(Smallfont, "" + numofTanks, new Vector2(displayWidth / 1.67f, displayHieght / 1.15f), Color.Black);
                }
                if (numofDIs > 0)
                {
                    spriteBatch.Draw(DisCounter, new Rectangle((int)(displayWidth / 1.73f), (int)(displayHieght / 1.097f), (int)(displayWidth / 18), (int)(displayHieght / 13)), Color.White);
                    spriteBatch.Draw(BlankTex, new Rectangle((int)(displayWidth / 1.61f), (int)(displayHieght / 1.09f), (int)(displayWidth / 150), (int)((averageDisHealth / 4) / numofDIs)), Color.Green);
                    spriteBatch.DrawString(Smallfont, "" + numofDIs, new Vector2(displayWidth / 1.67f, displayHieght / 1.06f), Color.Black);
                }


            #endregion

            #endregion

            #region selector
            //spriteBatch.Draw(BlankTex, stikeTar, Color.Red);
            if (waypointRec.Width != (displayWidth / 40))
            {
                if (setstrike) spriteBatch.Draw(DragBarTex, strikearea, Color.Red);
                else spriteBatch.Draw(waypointTex, waypointRec, Color.White);
            }
            if (buildnumber == 0)
            {
                //drag selecting
                if (selecting == true)
                {
                    spriteBatch.Draw(DragBarTex, DragBarRec, Color.White);
                    spriteBatch.Draw(dragIcon, CursorRec, Color.White);
                }
                if (CursorRec.X < (DragBarRec.X - (displayWidth / 60)) || CursorRec.Y < (DragBarRec.Y - (displayHieght / 60)))
                {
                    spriteBatch.Draw(NodragIcon, CursorRec, Color.White);
                }
                else
                {
                    //pick up tex
                    if (pickingup) spriteBatch.Draw(pickup, CursorRec, Color.White);
                    else
                    {
                        //reg cursor
                        if (selecting == false)
                        {
                            if (setstrike) spriteBatch.Draw(SelectorTex, CursorRec, Color.Red);
                            else spriteBatch.Draw(SelectorTex, CursorRec, Color.White);
                        }
                    }
                }
            }
            #endregion

            if (showBuildingbox || showUnitbox)
            {
                spriteBatch.Draw(BlankTex, new Rectangle(CursorRec.X, (int)(CursorRec.Y - (displayHieght / 11.5)), (int)(displayWidth / 10), (int)(displayHieght / 15)), Color.Gray);
                spriteBatch.DrawString(Regfont, "Rc: "+buildcost, new Vector2(CursorRec.X, CursorRec.Y - (displayHieght / 10)), Color.White);
                spriteBatch.DrawString(Regfont, "Bt: "+buildtime+"s", new Vector2(CursorRec.X, CursorRec.Y - (displayHieght / 15)), Color.White);
            }
            //spriteBatch.Draw(DragBarTex, LandUnitBoxCheck, Color.Red);
            //spriteBatch.Draw(DragBarTex, AirunitBoxCheck, Color.Blue);
        }

        /// <summary>
        /// does the calculations to set the waypoint of a unit
        /// </summary>
        public Rectangle waypointassigningCalculations(float unitwidth, float unithieght, int numofselected, int rowCornum)
        {
            Rectangle checkbox;

            if (numofselected <= rowCornum) { checkbox.Width = (int)(numofselected * (unitwidth * 1.5f)); }
            else { checkbox.Width = (int)(rowCornum * (unitwidth * 1.5f)); }

            if (numofselected % rowCornum == 0) { checkbox.Height = (int)((unithieght * 1.5) * (numofselected / rowCornum)); }
            else { checkbox.Height = (int)((unithieght * 1.5) * ((numofselected / rowCornum) + 1)); }

            checkbox.X = (CursorRec.X - (checkbox.Width / 2));
            checkbox.Y = (CursorRec.Y - (checkbox.Height / 2));

            if (checkbox.X < (mAp1.spriteRectangle.X + (displayWidth / 10))) checkbox.X = (int)(mAp1.spriteRectangle.X + (displayWidth / 10));
            if (checkbox.Right > (mAp1.spriteRectangle.Right - (displayWidth / 10))) checkbox.X = (int)((mAp1.spriteRectangle.Right - (displayWidth / 7)) - checkbox.Width);

            if (checkbox.Y < (mAp1.spriteRectangle.Y + (displayWidth / 10))) checkbox.Y = (int)(mAp1.spriteRectangle.Y + (displayWidth / 10));
            if (checkbox.Bottom > (mAp1.spriteRectangle.Bottom - (displayWidth / 10))) checkbox.Y = (int)((mAp1.spriteRectangle.Bottom - (displayWidth / 7)) - checkbox.Height);

            return checkbox;
        }

        /// <summary>
        /// creates human controled sprites
        /// </summary>
        /// <param name="spriteType">type of sprite 0-7 (flyer, tran, tank, airFac, lanFac, turret, areaEnhancer, Disruptor)</param>
        /// <param name="x">new sprite's x position</param>
        /// <param name="y">new sprite's y position</param>
        /// <param name="timeToBuild">time till a unit is built</param>
        public void createHumanSprite(int spriteType, int x, int y, int wayX, int wayY)
        {
            #region creates units
            if (spriteType == 0)
            {
                HumanFLyer hFlyer = new HumanFLyer(
                   Content.Load<Texture2D>("Humans/Units/HumanFlyerS"),
                   Content.Load<Texture2D>("Hud/HealthBar"),
                   .05f,
                   displayWidth, displayHieght,
                   x, y,
                   70, 70,
                   100, wayX, wayY,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"));

                HumanAirList.Add(hFlyer);
                Humans.Add(hFlyer);

                flyerbuilt.Play();
            }
            if (spriteType == 1)
            {
                HumanTransport hTransport = new HumanTransport(
                    Content.Load<Texture2D>("Humans/Units/HumanTransportS"),
                    .07f,
                    displayWidth, displayHieght,
                    x, y,
                    80, 80,
                    100,
                    Content.Load<Texture2D>("Humans/Units/DropOff"),
                    (HumanAirList.Count + 1), wayX, wayY,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"),
                   BlankTex, Content.Load<Texture2D>("Humans/Units/Distruptor"));

                HumanAirList.Add(hTransport);
                Humans.Add(hTransport);

                tranbuilt.Play();
            }
            if (spriteType == 2)
            {
                HumanTank hTank = new HumanTank(
                   Content.Load<Texture2D>("Humans/Units/HumanTank"),
                   Content.Load<Texture2D>("Hud/HealthBar"),
                   .03f,
                   displayWidth, displayHieght,
                   x, y,
                   150, 150,
                   100, wayX, wayY,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"));

                HumanLandList.Add(hTank);
                Humans.Add(hTank);

                tankBuilt.Play();
            }
            if (spriteType == 7)
            {
                Disruptor dis = new Disruptor(
                   Content.Load<Texture2D>("Humans/Units/Distruptor"),
                   Content.Load<Texture2D>("Humans/Units/Missle"),
                   Content.Load<Texture2D>("Humans/Units/Smoke"),
                   .05f,
                   displayWidth, displayHieght,
                   x, y,
                   100, 100,
                   100, wayX, wayY,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"));

                HumanLandList.Add(dis);
                Humans.Add(dis);

                disBuilt.Play();
            }
            #endregion
            #region creates buildings
            if (spriteType == 3)
            {
                HumanAirFactory hAirFact = new HumanAirFactory(
                    Content.Load<Texture2D>("Humans/Buildings/AirFactorystrip"),
                    Content.Load<Texture2D>("Hud/BuildTextures/BuildFLyerButton"),
                    Content.Load<Texture2D>("Hud/BuildTextures/BuildTransportButton"),
                    .09f,
                    displayWidth, displayHieght,
                    x, y,
                    102, 102, 100, 600,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"));

                HumanBuildingList.Add(hAirFact);
                Humans.Add(hAirFact);
            }
            if (spriteType == 4)
            {
                HumanLandFactory hLandFact = new HumanLandFactory(
                    Content.Load<Texture2D>("Humans/Buildings/HumanLandFactory"),
                    Content.Load<Texture2D>("Hud/BuildTextures/BuildTankButton"),
                    Content.Load<Texture2D>("Hud/BuildTextures/BuildDISButton"),
                    .09f,
                    displayWidth, displayHieght,
                    x, y,
                    100, 100, 100, 600,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"));

                HumanBuildingList.Add(hLandFact);
                Humans.Add(hLandFact);
            }
            if (spriteType == 5)
            {
                HumanTurret hTurret = new HumanTurret(
                    Content.Load<Texture2D>("Humans/Buildings/Turretstrip"),
                    BlankTex,
                    Content.Load<Texture2D>("Hud/HealthBar"),
                    .05f,
                    displayWidth, displayHieght,
                    x, y,
                    100, 100, 100, 360,
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"));

                HumanBuildingList.Add(hTurret);
                Humans.Add(hTurret);
            }
            if (spriteType == 6)
            {
                AreaEnhancer AreaEnhan = new AreaEnhancer(
                    Content.Load<Texture2D>("Humans/Buildings/AreaEnhancer"),
                    .06f,
                    displayWidth, displayHieght,
                    x, y,
                    100, 100, 100, 360,
                   Content.Load<Texture2D>("Hud/HealthBar"),
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"));

                HumanBuildingList.Add(AreaEnhan);
                buildAreasList.Add(AreaEnhan);
                Humans.Add(AreaEnhan);
            }
            #endregion
            skipUpdate = true;
        }
        #endregion

        #region AIHivemind

        //Random randSpawnAssigner = new Random();
        Random randsectionbuild = new Random();
        int randsectionbuildTimer;
        Random randbuilding = new Random();
        public int netNum;
        public int conNum;
        int randforce;

        #region infection stats
        int infectedTime;
        float PercentInfection;
        public int infectionHealthBoost;
        public int infectionDamageBoost;
        #endregion

        public void updateAIHIve()
        {
            KeyboardState key = Keyboard.GetState();

            #region networkbuilding

            if (randsectionbuildTimer == 0)
            {
                setnewRow(2);
                randsectionbuildTimer = randsectionbuild.Next(1200, 3600);
            }
            else randsectionbuildTimer--;

            #endregion

            #region send force

            if (randforce == 0)
            {
                foreach (Aiunit enemy in EnemyAirList)
                {
                    enemy.waypoint.X = (int)(mAp1.spriteRectangle.X + (displayWidth / 5));
                    enemy.waypoint.Y = (int)(mAp1.spriteRectangle.Y + (displayWidth / 5));
                    enemy.state = Aiunit.BaseUnitState.moving;
                }
                foreach (Aiunit enemy in EnemyLandList)
                {
                    enemy.waypoint.X = (int)(mAp1.spriteRectangle.X + (displayWidth / 5));
                    enemy.waypoint.Y = (int)(mAp1.spriteRectangle.Y + (displayWidth / 5));
                    enemy.state = Aiunit.BaseUnitState.moving;
                }
                randforce = randbuilding.Next(3600, 10800);
            }
            else
            {
                randforce--;
            }

            #endregion

            //demo
            if (key.IsKeyDown(Keys.A) && oldkeys.IsKeyUp(Keys.A)) createAISprite(0, mouse.X, mouse.Y, 100, 100);
            if (key.IsKeyDown(Keys.S) && oldkeys.IsKeyUp(Keys.S)) createAISprite(1, mouse.X, mouse.Y, 100, 100);
            if (key.IsKeyDown(Keys.D) && oldkeys.IsKeyUp(Keys.D)) createAISprite(2, mouse.X, mouse.Y, 100, 100);
            if (key.IsKeyDown(Keys.F) && oldkeys.IsKeyUp(Keys.F)) createAISprite(3, mouse.X, mouse.Y, 100, 100);
            if (key.IsKeyDown(Keys.G) && oldkeys.IsKeyUp(Keys.G)) createAISprite(4, mouse.X, mouse.Y, 100, 100);
            if (key.IsKeyDown(Keys.H) && oldkeys.IsKeyUp(Keys.H)) createAISprite(5, mouse.X, mouse.Y, 100, 100);
            if (key.IsKeyDown(Keys.J) && oldkeys.IsKeyUp(Keys.J)) createAISprite(6, mouse.X, mouse.Y, 100, 100);

            #region carbage man
            ////removes dead sprites
            foreach (baseSprite sprite in EnemyAirList)
            {
                if (sprite.dead)
                {
                    EnemyAirList.Remove(sprite);
                    Enemies.Remove(sprite);
                    break;
                }
            }
            foreach (baseSprite sprite in EnemyLandList)
            {
                if (sprite.dead)
                {
                    EnemyLandList.Remove(sprite);
                    Enemies.Remove(sprite);
                    break;
                }
            }
            foreach (Aibuilding sprite in EnemyBuildingList)
            {
                if (sprite.dead)
                {
                    if (sprite.indicator == "network")
                    {
                        //marks tenticals for removal
                        foreach (NetTentical tent in tenticalList)
                        {
                            if (tent.freQuency == sprite.netFrequency)
                            {
                                tent.lost = true;
                            }
                        }
                        //shuts down buildings
                        foreach (Aibuilding bui in EnemyBuildingList)
                        {
                            if (bui.netFrequency == sprite.netFrequency)
                            {
                                //bui.passedon = false;
                                bui.connected = false;
                            }
                        }
                    }
                    //reg garbage
                    foreach (NetTentical tent in tenticalList)
                    {
                        if (tent.designatedconnection == sprite.connectionFrequency)
                        {
                            tent.lost = true;
                        }
                    }
                    EnemyBuildingList.Remove(sprite);
                    Enemies.Remove(sprite);
                    break;
                }
            }
            foreach (NetTentical tent in tenticalList)
            {
                if (tent.lost)
                {
                    tenticalList.Remove(tent);
                    break;
                }
            }
            #endregion

            #region network information
            //if (key.IsKeyDown(Keys.OemMinus)) PercentInfection--;
            //if (key.IsKeyDown(Keys.OemPlus)) PercentInfection++;
            PercentInfection = EnemyBuildingList.Count;
            #endregion
        }

        public void drawAIHive()
        {
            foreach (NetTentical tentical in tenticalList)
            {
                tentical.drawtentical(spriteBatch);
            }
        }

        /// <summary>
        /// same as "createUnit", but for the AI
        /// </summary>
        /// <param name="spriteType">type of sprite 0-7 (Seeder(0), Swarm(1), Drifter(2),Spawner(3), Turret(4), Seed(5), network(6-7))</param>
        /// <param name="x">new sprite's x position</param>
        /// <param name="y">new sprite's y position</param>
        /// <param name="timeToBuild">time till a unit is built</param>
        public void createAISprite(int spriteType, int x, int y, int wayX, int wayY)
        {
            #region creates units

            if (spriteType == 0)
            {
                AISeeder seeder = new AISeeder(
                    Content.Load<Texture2D>("Aliens/Units/Seeder2"),
                    .15f, displayWidth, displayHieght,
                    x, y, 100, 100, 100 + infectionHealthBoost, wayX, wayY,
                   Content.Load<Texture2D>("Explosions/AlienExplosion"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"),
                   Content.Load<Texture2D>("Aliens/Units/seedAmmo"));

                EnemyLandList.Add(seeder);
                Enemies.Add(seeder);
            }
            if (spriteType == 1)
            {
                AISwarm swarm = new AISwarm(
                    Content.Load<Texture2D>("Aliens/Units/Swarm"),
                    .06f, displayWidth, displayHieght,
                    x, y, 100, 100, 50 + infectionHealthBoost, wayX, wayY,
                   Content.Load<Texture2D>("Explosions/AlienExplosion"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/hit-02"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"));

                EnemyLandList.Add(swarm);
                Enemies.Add(swarm);
            }
            if (spriteType == 2)
            {
                AIDrifter drifter = new AIDrifter(
                    Content.Load<Texture2D>("Aliens/Units/Drifter"),
                    Content.Load<Texture2D>("Hud/HealthBar"),
                    .1f, displayWidth, displayHieght,
                    x, y, 100, 100, 100 + infectionHealthBoost, wayX, wayY,
                   Content.Load<Texture2D>("Explosions/AlienExplosion"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<Texture2D>("Hud/Selector/DragBar"));

                EnemyAirList.Add(drifter);
                Enemies.Add(drifter);
            }

            #endregion
            #region creates buildings

            if (spriteType == 3)
            {
                AIspawner aispawner = new AIspawner(
                    Content.Load<Texture2D>("Aliens/Buildings/AISpawner"),
                    .1f, displayWidth, displayHieght,
                    x, y, 100, 100, 100 + infectionHealthBoost, 360,
                   Content.Load<Texture2D>("Explosions/AlienExplosion"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   DragBarTex, conNum);

                EnemyBuildingList.Add(aispawner);
                Enemies.Add(aispawner);

                conNum++;
            }
            if (spriteType == 4)
            {
                AIturret aiturret = new AIturret(
                    Content.Load<Texture2D>("Aliens/Buildings/AITurret"),
                    Content.Load<Texture2D>("Aliens/Buildings/NetTentical"),
                    .08f, displayWidth, displayHieght,
                    x, y, 100, 100, 100 + infectionHealthBoost, 180,
                   Content.Load<Texture2D>("Explosions/AlienExplosion"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   DragBarTex, conNum);

                EnemyBuildingList.Add(aiturret);
                Enemies.Add(aiturret);

                conNum++;
            }
            if (spriteType == 5)
            {
                AIseed aiseed = new AIseed(
                    Content.Load<Texture2D>("Aliens/Buildings/spawnpool"),
                    .08f, displayWidth, displayHieght,
                    x, y, 50, 50, 100 + infectionHealthBoost, 0,
                   Content.Load<Texture2D>("Explosions/AlienExplosion"),
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   DragBarTex, conNum, Regfont);

                EnemyBuildingList.Add(aiseed);
                Enemies.Add(aiseed);

                conNum++;
            }
            if (spriteType == 6 || spriteType == 7)
            {
                AINetwork net = new AINetwork(
                    Content.Load<Texture2D>("Aliens/Buildings/network"),
                    .1f, displayWidth, displayHieght,
                    x, y, 100, 100, 100 + infectionHealthBoost, 130,
                    Content.Load<Texture2D>("Explosions/AlienExplosion"),
                    Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                    DragBarTex, netNum + 1, conNum);

                EnemyBuildingList.Add(net);
                Enemies.Add(net);

                netNum++;
                conNum++;
            }
            #endregion
            skipUpdate = true;
        }

        /// <summary>
        /// creates a tenticle connection between buildings
        /// </summary>
        public void createConnection(int x, int y, int width, float rotation, int frequency, int designated)
        {
            NetTentical tentical = new NetTentical(
    Content.Load<Texture2D>("Aliens/Buildings/NetTentical"),
    width, displayHieght, x, y, rotation, frequency, designated, Regfont);

            tenticalList.Add(tentical);
        }

        /// <summary>
        /// sets up a new buildin connection for any eligable buildings
        /// </summary>
        public void setnewRow(int numofbuilds)
        {
            Random randpos = new Random();
            Random randbuild = new Random();
            //find the fathest point at which a building can connect
            int farthestup= 0;
            int farthestacross = 0;
            bool hasConnectedBuilding = false;
            foreach (Aibuilding oldbuild in EnemyBuildingList)
            {
                if (oldbuild.connected == true)
                {
                    hasConnectedBuilding = true;
                    if (farthestacross == 0)
                    {
                        farthestacross = oldbuild.NetRange.X;
                    }
                    else
                    {
                        if (oldbuild.NetRange.X < farthestacross)
                        {
                            farthestacross = oldbuild.NetRange.X;
                        }
                    }
                    if (farthestup == 0)
                    {
                        farthestup = oldbuild.NetRange.Y;
                    }
                    else
                    {
                        if (oldbuild.NetRange.Y < farthestup)
                        {
                            farthestup = oldbuild.NetRange.Y;
                        }
                    }
                }
            }
            if (hasConnectedBuilding)
            {
                List<Rectangle> newbuildings = new List<Rectangle>();
                for (int i = 0; i < numofbuilds; i++)
                {
                    //left -> right
                    Rectangle newbuild = new Rectangle(randpos.Next(farthestacross, (int)(mAp1.spriteRectangle.Right - (displayWidth / 5))),
                                                       randpos.Next(farthestup, (int)(farthestup + (displayWidth / 10))),
                                                       (int)(displayWidth * .1f), (int)(displayWidth * .1f));

                    foreach (Aibuilding oldbuild in EnemyBuildingList)
                    {
                        if (newbuild.Intersects(oldbuild.spriteRectangle))
                        {
                            if (newbuild.Center.X > oldbuild.spriteRectangle.Center.X)
                            {
                                newbuild.X = oldbuild.spriteRectangle.Right;
                            }
                            if (newbuild.Center.X < oldbuild.spriteRectangle.Center.X)
                            {
                                newbuild.X = oldbuild.spriteRectangle.X - newbuild.Width;
                            }
                            if (newbuild.Center.Y > oldbuild.spriteRectangle.Center.Y)
                            {
                                newbuild.Y = oldbuild.spriteRectangle.Bottom;
                            }
                            if (newbuild.Center.Y < oldbuild.spriteRectangle.Center.Y)
                            {
                                newbuild.Y = oldbuild.spriteRectangle.Y - newbuild.Height;
                            }
                        }
                    }
                    newbuildings.Add(newbuild);
                }
                for (int i = 0; i < numofbuilds; i++)
                {
                    //up -> down
                    Rectangle newbuild = new Rectangle(randpos.Next(farthestacross, (int)(farthestacross + (displayWidth / 10))),
                                                    randpos.Next(farthestup, (int)(mAp1.spriteRectangle.Bottom - (displayWidth / 5))),
                                                    (int)(displayWidth * .1f), (int)(displayWidth * .1f));

                    foreach (Aibuilding oldbuild in EnemyBuildingList)
                    {
                        if (newbuild.Intersects(oldbuild.spriteRectangle))
                        {
                            if (newbuild.Center.X > oldbuild.spriteRectangle.Center.X)
                            {
                                newbuild.X = oldbuild.spriteRectangle.Right;
                            }
                            if (newbuild.Center.X < oldbuild.spriteRectangle.Center.X)
                            {
                                newbuild.X = oldbuild.spriteRectangle.X - newbuild.Width;
                            }
                            if (newbuild.Center.Y > oldbuild.spriteRectangle.Center.Y)
                            {
                                newbuild.Y = oldbuild.spriteRectangle.Bottom;
                            }
                            if (newbuild.Center.Y < oldbuild.spriteRectangle.Center.Y)
                            {
                                newbuild.Y = oldbuild.spriteRectangle.Y - newbuild.Height;
                            }
                        }
                    }
                    newbuildings.Add(newbuild);
                }
                foreach (Rectangle newb in newbuildings)
                {
                    createAISprite(randbuild.Next(3, 7), newb.X, newb.Y, newb.X, newb.Y);
                }
            }
            #region old
            //foreach (Aibuilding newbuildhost in EnemyBuildingList)
            //{
            //    if (newbuildhost.connected) //&& newbuildhost.passedon == false)
            //    {
            //        Rectangle checkbox;
            //        if (newbuildhost.indicator == "network")
            //        {
            //            checkbox = new Rectangle(newbuildhost.NetRange.X, newbuildhost.NetRange.Y, (int)(displayWidth * .1f), (int)(displayWidth * .1f));
            //        }
            //        else
            //        {
            //            checkbox = new Rectangle(randpos.Next(newbuildhost.NetRange.X, newbuildhost.NetRange.Center.X), randpos.Next(newbuildhost.NetRange.Y, newbuildhost.NetRange.Center.Y),
            //                (int)(displayWidth * .1f), (int)(displayWidth * .1f));
            //        }

            //        foreach (Aibuilding blockage in EnemyBuildingList)
            //        {
            //            if (checkbox.Intersects(blockage.spriteRectangle))
            //            {
            //                if (checkbox.Y == newbuildhost.NetRange.Y)
            //                {
            //                    checkbox.X += checkbox.Width;
            //                }
            //                if (checkbox.Right > newbuildhost.NetRange.Right || checkbox.Right > (mAp1.spriteRectangle.Right - (displayWidth / 10)))
            //                {
            //                    checkbox.Y += checkbox.Height;
            //                    checkbox.X = newbuildhost.NetRange.X;
            //                }
            //                if (checkbox.Y != newbuildhost.NetRange.Y)
            //                {
            //                    checkbox.Y += checkbox.Height;
            //                }
            //                if (checkbox.Bottom > (mAp1.spriteRectangle.Bottom - (displayWidth / 10)))
            //                {
            //                    //newbuildhost.passedon = true;
            //                    break;
            //                }
            //            }
            //        }
            //        newbuildings.Add(checkbox);
            //    }
            //}
            //foreach (Rectangle buildarea in newbuildings)
            //{
            //    createAISprite(randbuild.Next(3, 5), buildarea.X, buildarea.Y, buildarea.X, buildarea.Y);
            //}

            #endregion
            skipUpdate = true;
        }
        #endregion

        #region pause/option screen

        Texture2D pausetex;
        Rectangle pauseRec;
        bool paused;
        Rectangle optionsbutton;
        Rectangle quitbutton;
        Rectangle controlsbutton;
        string branch;

        Texture2D controlsLayout;

        public bool showranges;
        public int difficultyMultiplier;
        public bool showHealths;

        public void loadPause()
        {
            //pause
            pausetex = Content.Load<Texture2D>("Hud/Pause");
            pauseRec = new Rectangle((int)(displayWidth - (displayWidth / 40)), 0, (int)displayWidth / 40, (int)displayWidth / 40);

            optionsbutton = new Rectangle((int)(displayWidth / 2.7f), (int)(displayHieght / 3.2f), (int)displayWidth / 4, (int)displayHieght / 20);
            controlsbutton = new Rectangle((int)(displayWidth / 2.7f), (int)(displayHieght / 2.55f), (int)displayWidth / 4, (int)displayHieght / 20);
            quitbutton = new Rectangle((int)(displayWidth / 2.7f), (int)(displayHieght / 2.12f), (int)displayWidth / 4, (int)displayHieght / 20);

            controlsLayout = Content.Load<Texture2D>("Hud/Controls");
        }

        public void UpdatePause()
        {
            //gameplay pause
            if (state == Gamestate.gameplay)
            {
                if (clicked && CursorRec.X > pauseRec.X && CursorRec.Y < pauseRec.Bottom ||
                    keys.IsKeyDown(Keys.P) && oldkeys.IsKeyUp(Keys.P) || pad.Buttons.Start == ButtonState.Pressed && oldpad.Buttons.Start == ButtonState.Released)
                {
                    if (paused) paused = false;
                    else
                    {
                        branch = "main";
                        paused = true;
                    }
                }
            }
            //switches branches of pause screen
            if (paused)
            {
                if (branch == "main")
                {
                    if (keys.IsKeyDown(Keys.B) && oldkeys.IsKeyUp(Keys.B) || pad.Buttons.B == ButtonState.Pressed && oldpad.Buttons.B == ButtonState.Released)
                    {
                        if (state == Gamestate.gameplay) paused = false;
                        else {state = Gamestate.galaticOverView; paused = false;}
                    }
                    //clicked options tab
                    if (clicked && CursorRec.Intersects(optionsbutton) && CursorRec.Y > optionsbutton.Y)
                    {
                        branch = "options";
                    }
                    //clicked controls tab
                    if (clicked && CursorRec.Intersects(controlsbutton) && CursorRec.Y > controlsbutton.Y)
                    {
                        branch = "controls";
                    }
                    //clicked quit tab
                    if (clicked && CursorRec.Intersects(quitbutton) && CursorRec.Y > quitbutton.Y)
                    {
                        if (state == Gamestate.gameplay)
                        {
                            paused = false;
                            startCleanup();
                            state = Gamestate.galaticOverView;
                        }
                        else
                        {
                            this.Exit();
                        }
                    }
                }
                if (branch == "options")
                {
                    if (keys.IsKeyDown(Keys.B) && oldkeys.IsKeyUp(Keys.B) || pad.Buttons.B == ButtonState.Pressed && oldpad.Buttons.B == ButtonState.Released) branch = "main";

                    if (CursorRec.Intersects(optionsbutton) && clicked && CursorRec.Y > optionsbutton.Y)
                    {
                        if (showranges) showranges = false;
                        else showranges = true;
                    }
                    if (CursorRec.Intersects(controlsbutton) && clicked && CursorRec.Y > controlsbutton.Y)
                    {
                        if (showHealths) showHealths = false;
                        else showHealths = true;
                    }
                    if (CursorRec.Intersects(quitbutton) && clicked && CursorRec.Y > quitbutton.Y)
                    {
                        if (difficultyMultiplier < 2) difficultyMultiplier++;
                        else difficultyMultiplier = 0;
                    }
                }
                if (branch == "controls")
                {
                    if (keys.IsKeyDown(Keys.B) && oldkeys.IsKeyUp(Keys.B) || pad.Buttons.B == ButtonState.Pressed && oldpad.Buttons.B == ButtonState.Released) branch = "main";
                }
            }
        }

        public void drawPause()
        {
            if (state == Gamestate.gameplay)spriteBatch.Draw(pausetex, pauseRec, Color.White);
            if (paused)
            {
                spriteBatch.Draw(BlankTex, new Rectangle(0, 0, (int)displayWidth, (int)displayHieght), new Color(0, 0, 0, 80));
                if (branch != "controls")
                {
                    spriteBatch.Draw(BlankTex, optionsbutton, Color.Gray);
                    spriteBatch.Draw(BlankTex, controlsbutton, Color.Gray);
                    spriteBatch.Draw(BlankTex, quitbutton, Color.Gray);
                }
                if (branch == "main")
                {
                    spriteBatch.DrawString(Regfont, "Paused", new Vector2(displayWidth / 2.22f, displayHieght / 5), Color.White);
                    spriteBatch.DrawString(Regfont, "OPTIONS", new Vector2(displayWidth / 2.25f, displayHieght / 3.25f), Color.White);
                    spriteBatch.DrawString(Regfont, "CONTROLS", new Vector2(displayWidth / 2.26f, displayHieght / 2.55f), Color.White);
                    if (state == Gamestate.gameplay) spriteBatch.DrawString(Regfont, "QUIT", new Vector2(displayWidth / 2.15f, displayHieght / 2.15f), Color.White);
                    else spriteBatch.DrawString(Regfont, "QUIT PROGRAM", new Vector2(displayWidth / 2.4f, displayHieght / 2.15f), Color.White);
                }
                if (branch == "options")
                {
                    //show ranges
                    spriteBatch.DrawString(Regfont, "Show Ranges:", new Vector2(displayWidth / 2.6f, displayHieght / 3.25f), Color.White);
                    if (showranges)
                    {
                        spriteBatch.DrawString(Regfont, "ON", new Vector2(displayWidth / 1.8f, displayHieght / 3.25f), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(Regfont, "OFF", new Vector2(displayWidth / 1.8f, displayHieght / 3.25f), Color.White);
                    }
                    //show healths
                    spriteBatch.DrawString(Regfont, "Show Health:", new Vector2(displayWidth / 2.6f, displayHieght / 2.56f), Color.White);
                    if (showHealths)
                    {
                        spriteBatch.DrawString(Regfont, "ON", new Vector2(displayWidth / 1.8f, displayHieght / 2.56f), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(Regfont, "OFF", new Vector2(displayWidth / 1.8f, displayHieght / 2.56f), Color.White);
                    }
                    //difficulty
                    spriteBatch.DrawString(Regfont, "Difficulty:", new Vector2(displayWidth / 2.6f, displayHieght / 2.15f), Color.White);
                    if (difficultyMultiplier == 0)
                    {
                        spriteBatch.DrawString(Regfont, "Easy", new Vector2(displayWidth / 1.86f, displayHieght / 2.15f), Color.White);
                    }
                    if (difficultyMultiplier == 1)
                    {
                        spriteBatch.DrawString(Regfont, "Medium", new Vector2(displayWidth / 1.86f, displayHieght / 2.15f), Color.White);
                    }
                    if (difficultyMultiplier == 2)
                    {
                        spriteBatch.DrawString(Regfont, "Hard", new Vector2(displayWidth / 1.86f, displayHieght / 2.15f), Color.White);
                    }
                }
                if (branch == "controls")
                {
                    spriteBatch.Draw(controlsLayout, new Rectangle(0, 0, (int)displayWidth, (int)displayHieght), Color.White);
                    spriteBatch.DrawString(Smallfont, "Exit    move     move       pause       cancel     enable stike     set         click: single select", new Vector2(0, displayHieght / 2), Color.Black);
                    spriteBatch.DrawString(Smallfont, "game   screen   cursor       game      selection    area for -->  waypoint      +drag: box select  ", new Vector2(0, displayHieght / 1.85f), Color.Black);
                    spriteBatch.DrawString(Smallfont, "Destroy selected buildings               2x click: all type select", new Vector2(displayWidth / 2.95f, displayHieght / 1.65f), Color.Black);
                }
                spriteBatch.DrawString(Regfont, "[B] back", new Vector2(0, 0), Color.Red);

                spriteBatch.Draw(SelectorTex, CursorRec, Color.White);
            }
        }
        #endregion

        #region creative
        /// <summary>
        /// creates smoke in all shapes and sizes.....and colors!
        /// </summary>
        public void disperseSmoke(int x, int y, float size, int dispersTime, Color color)
        {
            smoke smog = new smoke(
                Content.Load<Texture2D>("Humans/Units/Smoke"),
                (int)size,
                x, y, dispersTime,color);
            smokelist.Add(smog);
        }

        /// <summary>
        /// creates a missile with a size,speed,and target
        /// </summary>
        /// <param name="missileX">starting x</param>
        /// <param name="missileY">starting y</param>
        /// <param name="target">missile target</param>
        /// <param name="sizeX">width</param>
        /// <param name="sizeY">height</param>
        /// <param name="enemy">target species (network/humans)</param>
        public void launchMissile(int missileX, int missileY, Rectangle target, int sizeX, int sizeY, string enemy)
        {
            if (enemy == "network")
            {
                missiles Newmiss = new missiles(
                   Content.Load<Texture2D>("Humans/Units/Missle"),
                   missileX, missileY,
                   sizeX, sizeY,
                   target,
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"),
                   Content.Load<Texture2D>("Explosions/HumanExplosion2"), enemy);
                miss.Add(Newmiss);
            }
            if (enemy == "humans")
            {
                missiles Newmiss = new missiles(
                   Content.Load<Texture2D>("Aliens/Units/AlienMissle"),
                   missileX, missileY,
                   sizeX, sizeY,
                   target,
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<Texture2D>("Explosions/HumanExplosion2"), enemy);
                miss.Add(Newmiss);
            }
            if (enemy == "seed")
            {
                missiles Newmiss = new missiles(
                   Content.Load<Texture2D>("Aliens/Units/seedAmmo"),
                   missileX, missileY,
                   sizeX, sizeY,
                   target,
                   Content.Load<SoundEffect>("Explosions/AlienExplosionSound"),
                   Content.Load<Texture2D>("Explosions/AlienExplosion"), enemy);
                miss.Add(Newmiss);
            }
        }
        #endregion

        #region startMap

        public void startCleanup()
        {
            //clear humans
            Humans.Clear();
            HumanAirList.Clear();
            HumanBuildingList.Clear();
            HumanLandList.Clear();
            buildAreasList.Clear();
            //clear creative
            miss.Clear();
            smokelist.Clear();
            //clear enemies
            Enemies.Clear();
            EnemyAirList.Clear();
            EnemyBuildingList.Clear();
            EnemyLandList.Clear();
            tenticalList.Clear();
        }

        public void initailizeGame()
        {
            resouces = 200;
            mAp1.spriteRectangle.X = 0;
            mAp1.spriteRectangle.Y = 0;
            viewscreen = new Vector2(miniMap.X - 5, miniMap.Y - 7);
            PercentInfection = 0;

            //sets map music
            MediaPlayer.Stop();

            if (mapinfo == "1") MediaPlayer.Play(map1Song);
            if (mapinfo == "2") MediaPlayer.Play(map2Song);
            if (mapinfo == "3") MediaPlayer.Play(map3Song);
            if (mapinfo == "4") MediaPlayer.Play(map4Song);

            //sets alien start
            int startx = mAp1.spriteRectangle.Right - (int)((displayWidth * .12f) * 2);
            int starty = mAp1.spriteRectangle.Bottom - (int)((displayWidth * .12f) * 2);
            createAISprite(6, startx, starty, 0, 0);
            randsectionbuildTimer = randsectionbuild.Next(1200, 2400);

            //sets human start
            hCommand = new TempleOrResearch(
                Content.Load<Texture2D>("Humans/Buildings/HumanTemplestrip"),
                .15f,
                displayWidth, displayHieght,
                displayWidth / 8f, displayWidth / 8f,
                87, 87,
                100, 0,
                Content.Load<Texture2D>("Hud/HealthBar"),
                   Content.Load<Texture2D>("Explosions/HumanExplosion"),
                   Content.Load<SoundEffect>("Explosions/ExplosionSound"));

            HumanBuildingList.Add(hCommand);
            buildAreasList.Add(hCommand);
            Humans.Add(hCommand);
        }

        public void resetSongs()
        {
            MediaPlayer.Stop();
            if (cleardMap1 && cleardMap2 && cleardMap3 && cleardMap4) MediaPlayer.Play(VictorySong);
            else
            {
                MediaPlayer.Play(mainsong);
            }
        }
        #endregion

        public Gebieter()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (graphics.IsFullScreen == false)
            {
                graphics.ToggleFullScreen();
            }
            displayHieght = GraphicsDevice.Viewport.Height;
            displayWidth = GraphicsDevice.Viewport.Width;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainsong = Content.Load<Song>("BackgroundSongs/10 To Galaxy");
            map1Song = Content.Load<Song>("BackgroundSongs/11 Immaterial");
            map2Song = Content.Load<Song>("BackgroundSongs/14 Revival");
            map3Song = Content.Load<Song>("BackgroundSongs/16 A Coastal City");
            map4Song = Content.Load<Song>("BackgroundSongs/20 Dreary Stronghold");
            VictorySong = Content.Load<Song>("BackgroundSongs/05 Pinball Wizard");

            loadPause();
            title = Content.Load<Texture2D>("Backgrounds/BackgroundTitle");
            flare = Content.Load<Texture2D>("Hud/flare");
            flareRec = new Rectangle((int)(displayWidth / 1.12f), (int)(displayHieght / 5), (int)(displayWidth / 20), (int)(displayWidth / 20));

            galacticTex = Content.Load<Texture2D>("Backgrounds/GalacticOverview");
            Map1 = new Rectangle((int)(displayWidth / 3.75f), (int)(displayHieght/1.84f), (int)(displayWidth/12),(int)(displayWidth/12));
            Map2 = new Rectangle((int)(displayWidth / 4.8f), (int)(displayHieght / 1.53f), (int)(displayWidth / 40), (int)(displayWidth / 40));
            Map3 = new Rectangle((int)(displayWidth / 1.48f), (int)(displayHieght / 1.36f), (int)(displayWidth / 12), (int)(displayWidth / 12));
            Map4 = new Rectangle((int)(displayWidth / 1.64f), (int)(displayHieght / 10.4f), (int)(displayWidth / 12), (int)(displayWidth / 12));

            resourceError = Content.Load<SoundEffect>("Humans/Messages/tadErr00");

            mAp1 = new Map(
                Content.Load<Texture2D>("Backgrounds/Tile"),
                0, 0,
                displayWidth);

            hudLoad();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region mechanics
            if (startedSong == false)
            {
                MediaPlayer.Play(mainsong);
                startedSong = true;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                keys.IsKeyDown(Keys.Escape))
                this.Exit();

            pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsConnected == false)
            {
                keys = Keyboard.GetState();
                mouse = Mouse.GetState();
            }

            //cursor movement
            padx += (pad.ThumbSticks.Left.X * 7);
            pady += (pad.ThumbSticks.Left.Y * -7);

            if (padx < 0) padx = 0;
            if (padx > (displayWidth - displayWidth /40)) padx = (displayWidth - (displayWidth /40));
            if (pady < 0) pady = 0;
            if (pady > (displayHieght - (displayWidth/40))) pady = (displayHieght - (displayWidth/40));

            if (pad.IsConnected) CursorRec = new Rectangle((int)padx, (int)pady, (int)(displayWidth / 40), (int)(displayWidth / 40));
            else CursorRec = new Rectangle(mouse.X, mouse.Y, (int)(displayWidth / 40), (int)(displayWidth / 40));
            #endregion

            #region main game state
            //always pressuring growth
            infectedTime++;
            if (infectedTime > 6000)
            {
                infectionHealthBoost += 2;
                infectionDamageBoost += 2;
                infectedTime = 0;
            }
            #region clicking

            clicked = false;
            //checks if the mouse is trying to select
            if (mouse.LeftButton == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released || pad.Buttons.A == ButtonState.Pressed && oldpad.Buttons.A == ButtonState.Released)
            {
                clicked = true;
                if (clickcounter == 0) clickcounter = 15;
                else doubleclick = true;
            }
            if (clickcounter > 0) clickcounter--;
            else doubleclick = false;
            #endregion

            switch (state)
            {
                case Gamestate.title:
                    if (clicked||keys.IsKeyDown(Keys.A))
                    {
                        flarestart = true;
                    }
                    if (flarestart)
                    {
                        if (flareRec.Width < displayWidth * 2.5f)
                        {
                            flareRec.X -= 70;
                            flareRec.Y -= 70;
                            flareRec.Width += 140;
                            flareRec.Height += 140;
                        }
                        else
                            state = Gamestate.galaticOverView;
                    }
                    break;
                case Gamestate.galaticOverView:
                    if (CursorRec.Intersects(Map1)) mapinfo = "" + cleardMap1;//tile
                    if (CursorRec.Intersects(Map2)) mapinfo = "" + cleardMap2;//moon
                    if (CursorRec.Intersects(Map3)) mapinfo = "" + cleardMap3;//desert
                    if (CursorRec.Intersects(Map4)) mapinfo = "" + cleardMap4;//grass
                    //tranlates bool into infection status
                    if (mapinfo == "False") mapinfo = "Infected";
                    if (mapinfo == "True") mapinfo = "Purged";
                    if (clicked)
                    {
                        //assign map
                        if (CursorRec.Intersects(Map1) && cleardMap1 == false)
                        {
                            mapinfo = "1";
                            mAp1.spriteTexture = Content.Load<Texture2D>("Backgrounds/Tile");
                            initailizeGame();
                            state = Gamestate.gameplay;
                        }
                        if (CursorRec.Intersects(Map2) && cleardMap2 == false)
                        {
                            mapinfo = "2";
                            mAp1.spriteTexture = Content.Load<Texture2D>("Backgrounds/Moon");
                            initailizeGame();
                            state = Gamestate.gameplay;
                        }
                        if (CursorRec.Intersects(Map3) && cleardMap3 == false)
                        {
                            mapinfo = "3";
                            mAp1.spriteTexture = Content.Load<Texture2D>("Backgrounds/Desert");
                            initailizeGame();
                            state = Gamestate.gameplay;
                        }
                        if (CursorRec.Intersects(Map4) && cleardMap4 == false)
                        {
                            mapinfo = "4";
                            mAp1.spriteTexture = Content.Load<Texture2D>("Backgrounds/GrassLand");
                            initailizeGame();
                            state = Gamestate.gameplay;
                        }
                    }
                    if (keys.IsKeyDown(Keys.X) && oldkeys.IsKeyUp(Keys.X) || pad.Buttons.X == ButtonState.Pressed && oldpad.Buttons.X == ButtonState.Released)
                    {
                        paused = true;
                        branch = "main";
                        state = Gamestate.helpscreen;
                    }
                    break;
                case Gamestate.helpscreen:
                    UpdatePause();
                    break;
                case Gamestate.gameplay:
                    hudUpdate();
                    UpdatePause();
                    if (paused == false)
                    {
                        updateAIHIve();
                        #region moving Screen
                        //moves screen down
                        if (keys.IsKeyDown(Keys.Up) || pad.DPad.Up == ButtonState.Pressed)
                        {
                            if ((mAp1.spriteRectangle.Y - (displayWidth / 10)) < 0)
                            {
                                waypointRec.Y += 4;
                                CursorRec.Y += 4;
                                viewscreen.Y -= (.135f);
                                direction = "down";
                                mAp1.move(direction, false, 0, 0);
                                foreach (baseSprite buid in Humans)
                                {
                                    buid.move(direction, false, 0, 0);
                                }
                                foreach (baseSprite ene in Enemies)
                                {
                                    ene.move(direction, false, 0, 0);
                                }
                                foreach (NetTentical tent in tenticalList)
                                {
                                    tent.move(direction, false, 0, 0);
                                }
                                foreach (smoke smo in smokelist)
                                {
                                    smo.move(direction, false, 0, 0);
                                }
                                foreach (missiles mis in miss)
                                {
                                    mis.move(direction, false, 0, 0);
                                }
                            }
                        }
                        //moves screen up
                        if (keys.IsKeyDown(Keys.Down) || pad.DPad.Down == ButtonState.Pressed)
                        {
                            if ((mAp1.spriteRectangle.Bottom + (displayWidth / 10)) > displayHieght)
                            {
                                waypointRec.Y -= 4;
                                CursorRec.Y -= 4;
                                viewscreen.Y += (.135f);
                                direction = "up";
                                mAp1.move(direction, false, 0, 0);
                                foreach (baseSprite buid in Humans)
                                {
                                    buid.move(direction, false, 0, 0);
                                }
                                foreach (baseSprite ene in Enemies)
                                {
                                    ene.move(direction, false, 0, 0);
                                }
                                foreach (NetTentical tent in tenticalList)
                                {
                                    tent.move(direction, false, 0, 0);
                                }
                                foreach (smoke smo in smokelist)
                                {
                                    smo.move(direction, false, 0, 0);
                                }
                                foreach (missiles mis in miss)
                                {
                                    mis.move(direction, false, 0, 0);
                                }
                            }
                        }
                        //moves screen left
                        if (keys.IsKeyDown(Keys.Right) || pad.DPad.Right == ButtonState.Pressed)
                        {
                            if ((mAp1.spriteRectangle.Right + (displayWidth / 10)) > displayWidth)
                            {
                                waypointRec.X -= 4;
                                CursorRec.X -= 4;
                                viewscreen.X += (.135f);
                                direction = "left";
                                mAp1.move(direction, false, 0, 0);
                                foreach (baseSprite buid in Humans)
                                {
                                    buid.move(direction, false, 0, 0);
                                }
                                foreach (baseSprite ene in Enemies)
                                {
                                    ene.move(direction, false, 0, 0);
                                }
                                foreach (NetTentical tent in tenticalList)
                                {
                                    tent.move(direction, false, 0, 0);
                                }
                                foreach (smoke smo in smokelist)
                                {
                                    smo.move(direction, false, 0, 0);
                                }
                                foreach (missiles mis in miss)
                                {
                                    mis.move(direction, false, 0, 0);
                                }
                            }
                        }
                        //moves screen right
                        if (keys.IsKeyDown(Keys.Left) || pad.DPad.Left == ButtonState.Pressed)
                        {
                            if ((mAp1.spriteRectangle.X - (displayWidth / 10)) < 0)
                            {
                                waypointRec.X += 4;
                                CursorRec.X += 4;
                                viewscreen.X -= (.135f);
                                direction = "right";
                                mAp1.move(direction, false, 0, 0);
                                foreach (baseSprite buid in Humans)
                                {
                                    buid.move(direction, false, 0, 0);
                                }
                                foreach (baseSprite ene in Enemies)
                                {
                                    ene.move(direction, false, 0, 0);
                                }
                                foreach (NetTentical tent in tenticalList)
                                {
                                    tent.move(direction, false, 0, 0);
                                }
                                foreach (smoke smo in smokelist)
                                {
                                    smo.move(direction, false, 0, 0);
                                }
                                foreach (missiles mis in miss)
                                {
                                    mis.move(direction, false, 0, 0);
                                }
                            }
                        }
                        #endregion

                        if (smokeletout == 0)
                        {
                            foreach (missiles mis in miss)
                            {
                                if (mis.Enemy == "network") disperseSmoke((int)(mis.rec.X - displayWidth / 100), (int)(mis.rec.Y - (displayWidth / 100)), displayWidth / 100, 150, Color.Gray);
                                if (mis.Enemy == "humans") disperseSmoke((int)(mis.rec.X - displayWidth / 100), (int)(mis.rec.Y - (displayWidth / 100)), displayWidth / 100, 150, Color.Red);
                            }
                            smokeletout = 15;
                        }
                        else smokeletout--;

                        #region updates classes
                        //buildings
                        foreach (baseSprite human in HumanBuildingList)
                        {
                            human.Update(this);
                        }
                        foreach (baseSprite ai in EnemyBuildingList)
                        {
                            ai.Update(this);
                        }
                        //units
                        foreach (baseSprite human in HumanAirList)
                        {
                            human.Update(this);
                        }
                        foreach (baseSprite human in HumanLandList)
                        {
                            human.Update(this);
                        }
                        foreach (baseSprite ai in EnemyAirList)
                        {
                            ai.Update(this);
                        }
                        foreach (baseSprite ai in EnemyLandList)
                        {
                            ai.Update(this);
                        }
                        //smoke
                        foreach (smoke smok in smokelist)
                        {
                            smok.Update(this);
                        }
                        //missles
                        foreach (missiles mis in miss)
                        {
                            mis.Update(this);
                        }
                        #endregion

                        if (PercentInfection == 100)
                        {
                            Victory = false;
                            resetSongs();
                            state = Gamestate.postgame;
                        }
                        if (PercentInfection == 0)
                        {
                            if (mapinfo == "1") cleardMap1 = true;
                            if (mapinfo == "2") cleardMap2 = true;
                            if (mapinfo == "3") cleardMap3 = true;
                            if (mapinfo == "4") cleardMap4 = true;
                            Victory = true;
                            resetSongs();
                            state = Gamestate.postgame;
                        }
                    }
                    break;
                case Gamestate.postgame:
                    startCleanup();
                    if (keys.IsKeyDown(Keys.A) || pad.Buttons.A == ButtonState.Pressed)
                    {
                        if (cleardMap1 && cleardMap2 && cleardMap3 && cleardMap4) this.Exit();
                        state = Gamestate.galaticOverView;
                    }
                    break;
            }
            #endregion

            oldkeys = keys;
            if (pad.IsConnected)oldpad = pad;
            else oldmouse = mouse;
            skipUpdate = false;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (state)
            {
                case Gamestate.title:
                    spriteBatch.Draw(title, new Rectangle(0,0,(int)displayWidth,(int)displayHieght), Color.White); 
                    spriteBatch.Draw(flare, flareRec, new Color(220,220,250,150));
                    spriteBatch.DrawString(Regfont, "Press [A] or click to Play", new Vector2(displayWidth/3,displayHieght/1.3f), Color.White);
                    break;
                case Gamestate.galaticOverView:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Draw(galacticTex, new Rectangle(0, 0, (int)displayWidth, (int)displayHieght), Color.White);
                    spriteBatch.DrawString(Regfont,"Infection Adaption Time: " + (infectedTime/60),new Vector2(0,displayHieght/28),Color.Red);
                    spriteBatch.DrawString(Regfont,"Infection Health: +"+infectionHealthBoost, new Vector2(0,displayHieght/15), Color.Red);
                    spriteBatch.DrawString(Regfont,"Infection Damage: +"+infectionDamageBoost, new Vector2(0,displayHieght/10), Color.Red);
                    spriteBatch.DrawString(Regfont, "[X] Options", new Vector2(displayWidth/1.2f,displayHieght/1.05f), Color.Red);
                    spriteBatch.Draw(SelectorTex, CursorRec, Color.White);
                    //map info
                    if (CursorRec.Intersects(Map1) || CursorRec.Intersects(Map2) || CursorRec.Intersects(Map3) || CursorRec.Intersects(Map4))
                    {
                        spriteBatch.Draw(BlankTex, new Rectangle(CursorRec.X, CursorRec.Y, (int)(displayWidth / 9), (int)(displayHieght / 30)), Color.Gray);
                        if (mapinfo == "Infected") spriteBatch.DrawString(Regfont, mapinfo, new Vector2(CursorRec.X, CursorRec.Y - (displayHieght / 80)), Color.Red);
                        if (mapinfo == "Purged") spriteBatch.DrawString(Regfont, mapinfo, new Vector2(CursorRec.X, CursorRec.Y - (displayHieght / 80)), Color.Blue);
                    }
                        //temp
                    //spriteBatch.Draw(BlankTex, Map1, Color.White);
                    //spriteBatch.Draw(BlankTex, Map2, Color.White);
                    //spriteBatch.Draw(BlankTex, Map3, Color.White);
                    //spriteBatch.Draw(BlankTex, Map4, Color.White);

                    break;
                case Gamestate.helpscreen:
                    GraphicsDevice.Clear(Color.Black);
                    drawPause();
                    spriteBatch.Draw(SelectorTex, CursorRec, Color.White);
                    break;
                case Gamestate.gameplay:
                    mAp1.Draw(spriteBatch);
                    drawAIHive();
                    //land
                    foreach (baseSprite sprite in HumanLandList)
                    {
                        sprite.Draw(spriteBatch);
                    }
                    foreach (baseSprite enemy in EnemyLandList)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    //buildings
                    foreach (baseSprite sprite in HumanBuildingList)
                    {
                        sprite.Draw(spriteBatch);
                    }
                    foreach (baseSprite enemy in EnemyBuildingList)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    //air
                    foreach (baseSprite sprite in HumanAirList)
                    {
                        sprite.Draw(spriteBatch);
                    }
                    //smoke
                    foreach (smoke smok in smokelist)
                    {
                        smok.Draw(spriteBatch);
                    }
                    foreach (baseSprite enemy in EnemyAirList)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    //missiles
                    foreach (missiles mis in miss)
                    {
                        mis.Draw(spriteBatch);
                    }
                    hudDraw();
                    drawPause();
                    break;
                case Gamestate.postgame:
                    if (cleardMap1 && cleardMap2 && cleardMap3 && cleardMap4)
                    {
                        GraphicsDevice.Clear(Color.Blue);
                        spriteBatch.DrawString(TitleFont, "System Cleared", new Vector2(displayWidth / 7, displayHieght / 3), Color.Gray);
                    }
                    else
                    {
                        if (Victory)
                        {
                            GraphicsDevice.Clear(Color.Blue);
                            spriteBatch.DrawString(TitleFont, "Planet Cleared", new Vector2(displayWidth / 7, displayHieght / 3), Color.Gray);
                        }
                        else
                        {
                            GraphicsDevice.Clear(Color.Black);
                            spriteBatch.DrawString(TitleFont, "Failure:", new Vector2(displayWidth / 3.5f, displayHieght / 4), Color.Red);
                            spriteBatch.DrawString(Regfont, "Contamination spreading", new Vector2(displayWidth / 3, displayHieght / 2), Color.Red);
                        }
                    }
                    spriteBatch.DrawString(Regfont, "(A) to continue", new Vector2(displayWidth / 2.6f, displayHieght / 1.1f), Color.White);
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}