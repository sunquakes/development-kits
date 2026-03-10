param(
    [string]$sourcePng = "e:\ruixinglong\development-kits\Resources\Images\logo.png",
    [string]$outputIco = "e:\ruixinglong\development-kits\Resources\Images\logo.ico"
)

# Check if ImageMagick is installed
$magickPath = Get-Command magick -ErrorAction SilentlyContinue

if ($magickPath) {
    Write-Host "Using ImageMagick to convert..." -ForegroundColor Green
    
    # Create multi-size ICO
    & magick convert $sourcePng -define icon:auto-resize=256,128,96,64,48,32,16 $outputIco
    
    Write-Host "ICO file created: $outputIco" -ForegroundColor Green
} else {
    Write-Host "ImageMagick not found. Trying alternative method..." -ForegroundColor Yellow
    
    # Use built-in .NET method (simpler approach)
    Add-Type -AssemblyName System.Drawing
    Add-Type -AssemblyName System.Windows.Forms
    
    try {
        $png = [System.Drawing.Image]::FromFile($sourcePng)
        Write-Host "Source PNG: $($png.Width)x$($png.Height)" -ForegroundColor Green
        
        # Create 256x256 icon (largest standard size)
        $bitmap = New-Object System.Drawing.Bitmap(256, 256)
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        
        $graphics.DrawImage($png, 0, 0, 256, 256)
        
        $icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
        
        $fileStream = New-Object System.IO.FileStream($outputIco, [System.IO.FileMode]::Create)
        $icon.Save($fileStream)
        $fileStream.Close()
        
        Write-Host "ICO file created: $outputIco (256x256)" -ForegroundColor Green
        
        $icon.Dispose()
        $bitmap.Dispose()
        $graphics.Dispose()
        $png.Dispose()
        
    } catch {
        Write-Host "Error: $_" -ForegroundColor Red
        Write-Host "`nPlease install ImageMagick or use an online converter:" -ForegroundColor Yellow
        Write-Host "  - https://icoconvert.com/" -ForegroundColor Cyan
        Write-Host "  - https://convertio.co/png-ico/" -ForegroundColor Cyan
    }
}
