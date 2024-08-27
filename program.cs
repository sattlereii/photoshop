using System;
using System.Drawing;
using System.Windows.Forms;

public class ImageEditor : Form
{
    private PictureBox pictureBox;
    private TrackBar brightnessBar;
    private TrackBar contrastBar;
    private Button uploadButton;
    private Button saveButton;
    private CheckBox redFilter;
    private CheckBox blueFilter;
    private CheckBox yellowFilter;
    private CheckBox greenFilter;
    private CheckBox orangeFilter;
    private Bitmap originalImage;

    public ImageEditor()
    {
        pictureBox = new PictureBox { Dock = DockStyle.Top, Height = 400 };
        brightnessBar = new TrackBar { Minimum = -100, Maximum = 100, Dock = DockStyle.Top };
        contrastBar = new TrackBar { Minimum = -100, Maximum = 100, Dock = DockStyle.Top };
        uploadButton = new Button { Text = "Nahrát obrázek", Dock = DockStyle.Top };
        saveButton = new Button { Text = "Uložit obrázek", Dock = DockStyle.Top };
        redFilter = new CheckBox { Text = "Červený filtr", Dock = DockStyle.Top };
        blueFilter = new CheckBox { Text = "Modrý filtr", Dock = DockStyle.Top };
        yellowFilter = new CheckBox { Text = "Žlutý filtr", Dock = DockStyle.Top };
        greenFilter = new CheckBox { Text = "Zelený filtr", Dock = DockStyle.Top };
        orangeFilter = new CheckBox { Text = "Oranžový filtr", Dock = DockStyle.Top };

        Controls.Add(orangeFilter);
        Controls.Add(greenFilter);
        Controls.Add(yellowFilter);
        Controls.Add(blueFilter);
        Controls.Add(redFilter);
        Controls.Add(saveButton);
        Controls.Add(contrastBar);
        Controls.Add(brightnessBar);
        Controls.Add(uploadButton);
        Controls.Add(pictureBox);

        uploadButton.Click += UploadButton_Click;
        saveButton.Click += SaveButton_Click;
        brightnessBar.Scroll += BrightnessBar_Scroll;
        contrastBar.Scroll += ContrastBar_Scroll;
        redFilter.CheckedChanged += Filter_CheckedChanged;
        blueFilter.CheckedChanged += Filter_CheckedChanged;
        yellowFilter.CheckedChanged += Filter_CheckedChanged;
        greenFilter.CheckedChanged += Filter_CheckedChanged;
        orangeFilter.CheckedChanged += Filter_CheckedChanged;
    }

    private void UploadButton_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            originalImage = new Bitmap(openFileDialog.FileName);
            pictureBox.Image = new Bitmap(originalImage);
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (pictureBox.Image == null)
        {
            MessageBox.Show("Nejprve nahrajte a upravte obrázek.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp";
        saveFileDialog.Title = "Uložit upravený obrázek";

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            pictureBox.Image.Save(saveFileDialog.FileName);
        }
    }

    private void BrightnessBar_Scroll(object sender, EventArgs e)
    {
        ApplyImageChanges();
    }

    private void ContrastBar_Scroll(object sender, EventArgs e)
    {
        ApplyImageChanges();
    }

    private void Filter_CheckedChanged(object sender, EventArgs e)
    {
        ApplyImageChanges();
    }

    private void ApplyImageChanges()
    {
        if (originalImage == null)
            return;

        Bitmap adjustedImage = AdjustBrightnessAndContrast(new Bitmap(originalImage), brightnessBar.Value, contrastBar.Value);
        adjustedImage = ApplyColorFilter(adjustedImage);
        pictureBox.Image = adjustedImage;
    }

    private Bitmap AdjustBrightnessAndContrast(Bitmap image, int brightness, int contrast)
    {
        Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);

                int newRed = Clamp(pixelColor.R + brightness, 0, 255);
                int newGreen = Clamp(pixelColor.G + brightness, 0, 255);
                int newBlue = Clamp(pixelColor.B + brightness, 0, 255);

                newRed = Clamp((int)(newRed * (contrast / 50.0)), 0, 255);
                newGreen = Clamp((int)(newGreen * (contrast / 50.0)), 0, 255);
                newBlue = Clamp((int)(newBlue * (contrast / 50.0)), 0, 255);

                adjustedImage.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
            }
        }

        return adjustedImage;
    }

    private Bitmap ApplyColorFilter(Bitmap image)
    {
        Bitmap filteredImage = new Bitmap(image.Width, image.Height);

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);

                if (redFilter.Checked)
                    pixelColor = Color.FromArgb(pixelColor.R, 0, 0);
                if (blueFilter.Checked)
                    pixelColor = Color.FromArgb(0, 0, pixelColor.B);
                if (yellowFilter.Checked)
                    pixelColor = Color.FromArgb(pixelColor.R, pixelColor.G, 0);
                if (greenFilter.Checked)
                    pixelColor = Color.FromArgb(0, pixelColor.G, 0);
                if (orangeFilter.Checked)
                    pixelColor = Color.FromArgb(pixelColor.R, (int)(pixelColor.G * 0.5), 0);

                filteredImage.SetPixel(x, y, pixelColor);
            }
        }

        return filteredImage;
    }

    private int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    [STAThread]
    public static void Main()
    {
        Application.Run(new ImageEditor());
    }
}
