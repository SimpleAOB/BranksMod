using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BranksMod
{
    public partial class DocFrm : Form
    {
        Color ThemeBackground = Color.FromArgb(0, 0, 0);
        Color ThemeHighlight = Color.FromArgb(0, 0, 0);
        Color ThemeFontColor = Color.FromArgb(0, 0, 0);

        public DocFrm()
        {
            InitializeComponent();
        }

        private void DocFrm_Load(object sender, EventArgs e)
        {
            this.Text = "BranksMod - " + Controller.Documentation;
            LoadTheme();
            LoadDocs();
        }

        public void LoadDocs()
        {
            //CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "";
            DescriptionBox.Text = null;
            CommandsBox.Text = null;
            NotesBox.Text = null;
            NotesTitle.Visible = false;
            CommandsTitle.Text = "Commands";
            if (Controller.Documentation == Controller.Plugins[0])
            {
                TitleLbl.Text = "AirRecoveryPlugin";
                DescriptionBox.Text = "BakkesMod plugin intended to practice recovery in the air.";
                CommandsBox.Text = "airrecovery_start - Starts airrecovery minigame.";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "airrecovery_stop - Stops airrecovery minigame.";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "airrecovery_bumpspeed_angular - Sets the strength of the torque applied to the car in a bump.";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "airrecovery_bumpspeed_linear - Set the strength of the horizontal force aplied to the car in a bump.";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "airrecovery_bumpspeed_linear_z - Set the strength of the vertical force aplied to the car in a bump. (Negative values=down, positive=up)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "airrecovery_cooldown - Set the time between bumps in ms.";
            }
            else if (Controller.Documentation == Controller.Plugins[1])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "BakkesMod Curveball Plugin";
                DescriptionBox.Text = "Adds Magnus force and drag to the ball in Rocket League.";
                CommandsBox.Text = "sv_soccar_curve 0|1 -- turn the effect on and off";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_lift_coefficient <float> -- affects how much the curve turns the ball";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_lift_zmod <float> -- multiplier for the vertical curving";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_maxspin <float> -- multiplier for the maximum spin speed the ball can have";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_lift_ground 0|1 -- turns horizontal curving on when the ball is on the ground";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_draw_magnus_force 0|1 -- draw some little red lines showing which way the magnus force is pushing the ball";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_magnus_debug 0|1 -- when activated, autospins the ball very fast, to ensure you see a visible effect on the ball";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "sv_soccar_forcemode - 0|6 -- specify the method of adding force to the ball";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "Force               = 0,";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "Impulse             = 1,";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "Velocity            = 2,";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "SmoothImpulse       = 3,";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "SmoothVelocity      = 4,";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "Acceleration        = 5,";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "MAX                 = 6";
                NotesBox.Text = "When compiling the code; check and modify the include directories to point to the BakkesMod sdk, and the post-build step as well.";
            }
            else if (Controller.Documentation == Controller.Plugins[2])
            {
                TitleLbl.Text = "CirclePlugin";
                DescriptionBox.Text = "Custom plugin for BakkesMod that allows you to measure how fast you can dribble in a circle. In other words, how fast you can turn the ball around.";
                CommandsBox.Text = "circle_start - Start measurement";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "circle_stop - Abort measurement";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "circle_duration - Set the duration of the measurement (default: 60)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "circle_gui hide|show - Hide/show GUI";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "circle_gui_x [coordinate] - X origin of GUI from right border of the screen in pixels (default: 500)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "circle_gui_y [coordinate] - Y origin of GUI from the top of the screen in pixels (default: 200)";
            }
            else if (Controller.Documentation == Controller.Plugins[3])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "DiscordRPCPlugin";
                DescriptionBox.Text = "Custom plugin for BakkesMod that integrates Discord Rich Presence into Rocket League.";
                CommandsBox.Text = "rpc_enabled 1|0 - Enable Discord RPC (Default: 1)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "rpc_show_online_games 1|0 - Should show online games in Rich Presence (Default: 1)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "rpc_show_replays 1|0 - Should show replay viewing in Rich Presence (Default: 1)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "rpc_show_freeplay 1|0 - Should show freeplay in Rich Presence (Default: 1)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "rpc_show_training 1|0 - Should show custom training in Rich Presence (Default: 1)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "rpc_show_spectating 1|0 - Should show spectating online games in Rich Presence (Default: 1)";
                NotesBox.Text = "If you are going to modify and compile on your own, keep in mind this requires the win32-static discord-rpc SDK. You should also change the include and lib locations to point to the BakkesMod SDK and discord-rpc SDK.";
            }
            else if (Controller.Documentation == Controller.Plugins[4])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "GravityPlugin";
                DescriptionBox.Text = "Custom gravity plugin for BakkesMod. Allows changing gravity in any direction. (Can also change strength)";
                CommandsBox.Text = "gravity_on [0|1] - turn custom gravity on/off";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "gravity_strength - Set the gravity strength (default: 650)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "gravity_ball_strength - Multiplier for ball gravity relative to car (default: 1.0)";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "gravity_polar - Set the polar angle for the gravity direction.";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "gravity_azimuth - Set the azimuth angle for the gravity direction.";
                NotesBox.Text = "If you don't understand these angles, check out https://en.wikipedia.org/wiki/Spherical_coordinate_system";
            }
            else if (Controller.Documentation == Controller.Plugins[5])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "BakkesMod Hitbox Plugin";
                DescriptionBox.Text = "Draws a hitbox around your car during training. Car type is auto-detected, and can also be set manually.";
                CommandsBox.Text = "cl_soccar_showhitbox [0|1] - turn hitbox on/off";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "cl_soccar_listhitboxtypes - see a list of the integer values for different car bodies. Use this as a parameter to cl_soccar_sethitboxtype";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "cl_soccar_sethitboxtype - set the hitbox to the specified value (e.g. 'cl_soccar_sethitboxtype 803' for Batmobile).";
                NotesBox.Text = "When compiling the code; check and modify the include directories to point to the BakkesMod sdk, and the post-build step as well.";
            }
            else if (Controller.Documentation == Controller.Plugins[6])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "Instant Training";
                DescriptionBox.Text = "This is the InstantTraining plugin for Rocket League, more specifically, BakkesMod. It immediately puts the player in training, as soon as a match is over.";
                CommandsBox.Text = @"Open up <steam_directory>\steamapps\common\rocketleague\Binaries\Win32\bakkesmod\cfg\plugins.cfg with notepad or a similar text editing application. Append plugin load InstantTraining to the bottom of the file and save it. t should look something like this when you are done:";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load defenderplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load dribbleplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load reboundplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load redirectplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load trainingplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load rconplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load airdribbleplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load workshopplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load wallhitplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load mechanicalplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load dollycamplugin2";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load commandexecutor";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load recoveryplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load ballpredictionplugin";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load autoreplayuploader";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "plugin load InstantTraining";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "Now the plugin will be loaded everytime you load Bakkesmod. You can easily edit the settings by going to the Plugins tab in BakkesMod. There you can edit the settings for the InstantTraining plugin.";
                NotesBox.Text = "Delay - If you do not want to join freeplay instantly, you can set a custom delay (in seconds) to wait before joining";
                NotesBox.Text = NotesBox.Text + Environment.NewLine + "AutoGG Support - If you use the built-in Automatically say GG feature, no need to worry, you will get your GG in before joining freeplay";
            }
            else if (Controller.Documentation == Controller.Plugins[7])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "MrSuluPlugin";
                DescriptionBox.Text = "BakkesMod plugin for RocketLeague.";
                CommandsBox.Text = "This plugin will display (in the console, press F6) the time elapsed of your rides to hit the world, or hit the ball or score. Can be enabled in freeplay and custom trainings. Very handy to train being as fast as possible (fast aerials, drive without boost, ...).";
                NotesBox.Text = "Compile the plugin as a 32 bit dll with bakkesmodsdk ( instructions http://bakkesmod.com/sdk.php ) If you don't want to compile it, there's a dll available in /Release directory.";
            }
            else if (Controller.Documentation == Controller.Plugins[8])
            {
                CommandsTitle.Text = "Additional Notes";
                TitleLbl.Text = "ReplayStatsBox";
                DescriptionBox.Text = "This plugin adds the statistics box to replays. Normally the box is only seen in the bottom left corner when spectating a live game and is not available in replays.";
                CommandsBox.Text = "If you want this plugin to automatically run, add plugin load replaystatsbox to /bakkesmod/cfg/plugins.cfg To get to that folder: in the bakkesmod injector click File > Open BakkesMod folder, then you will see the cfg folder";
            }
            else if (Controller.Documentation == Controller.Plugins[9])
            {
                NotesTitle.Visible = true;
                TitleLbl.Text = "SciencePlugin";
                DescriptionBox.Text = "Plugin for BakkesMod that can be used to perform scientific experiments within Rocket League. It only works in freeplay and is not meant to be used outside of that.";
                CommandsBox.Text = "To show/hide panels, use the settings in the bakkesmod menu (F2) (only works when plugin loaded on start-up). Alternatively, use the showHUD/hideHUD commands.";
                CommandsBox.Text = CommandsBox.Text + Environment.NewLine + "To record e.g. car data, set recordCarInfo to 1, and to 0 to stop recording. I recommend adding hotkeys for these commands such that recording and stopping can be done with simple button presses.";
                NotesBox.Text = "Real-time displaying of information about the car and/or the ball, such as location, velocity and rotation, as well as player controller inputs.";
                NotesBox.Text = NotesBox.Text + Environment.NewLine + "Recording of RBState data of car and/or ball.";
                NotesBox.Text = NotesBox.Text + Environment.NewLine + "Overriding player input (limited).";
                NotesBox.Text = NotesBox.Text + Environment.NewLine + "Bunch of setters and getters";
            }
        }

        public void LoadTheme()
        {
            if (Properties.Settings.Default.Theme == "Light")
            {
                ThemeBackground = Color.FromArgb(240, 240, 240);
                ThemeHighlight = Color.FromArgb(255, 255, 255);
                ThemeFontColor = Color.FromArgb(5, 5, 5);
            }
            else if (Properties.Settings.Default.Theme == "Night")
            {
                ThemeBackground = Color.FromArgb(40, 40, 40);
                ThemeHighlight = Color.FromArgb(55, 55, 55);
                ThemeFontColor = Color.FromArgb(250, 250, 250);
            }
            this.BackColor = ThemeHighlight;
            TitleLbl.ForeColor = ThemeFontColor;
            CommandsTitle.ForeColor = ThemeFontColor;
            NotesTitle.ForeColor = ThemeFontColor;
            DescriptionBox.BackColor = ThemeHighlight;
            CommandsBox.BackColor = ThemeHighlight;
            NotesBox.BackColor = ThemeHighlight;
            DescriptionBox.ForeColor = ThemeFontColor;
            CommandsBox.ForeColor = ThemeFontColor;
            NotesBox.ForeColor = ThemeFontColor;
        }

    }
}
