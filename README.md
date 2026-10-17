# V9 Pro Control

A small native Windows app that controls an **MCHOSE V9 PRO** wireless headset, written to
replace the vendor's MCHOSE HUB.

## Why

MCHOSE HUB works, but it is an Electron app that fetches its entire interface from
`cdn.mchose.com.cn` at startup, reports telemetry, and keeps background watcher processes
alive. Every setting it changes is in fact a local Windows audio property, so none of that is
necessary. This app does the same work in a single ~200 KB executable with no network access.

## What it does

| Feature | Notes |
| --- | --- |
| Output and microphone volume | Plus microphone mute |
| 10-band equalizer | 20 Hz – 20 kHz, ±20 dB, draggable curve |
| Audio brightening | On/off plus intensity |
| Vocal clarity | On/off, intensity and noise suppression |
| Dynamic low frequency | On/off, intensity and band segmentation in Hz |
| Intelligent volume | On/off, intensity and dynamics mode |
| Battery level | Read from the headset on demand |
| Sound mode | Game 1, Game 2, Music |

Everything applies immediately, exactly as it does in the Hub, and the Hub will show the same
values if you open it afterwards.

## How it works

The headset's DSP settings are not kept on the device. They live in the Windows audio
endpoint's property store, under:

```
HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render\{endpoint}\FxProperties
```

Each effect is a property set GUID plus a numeric index. The layout is not documented
anywhere, so it was mapped by diffing that key while toggling one control at a time in the
Hub. `src/Apo/Equalizer.cs` and `src/Apo/PlaybackEffects.cs` record what each index means.

Writing the registry alone only persists a value; the running APO does not re-read it until
the audio stream restarts. To apply changes live the app calls `PropertyControl` in the
vendor's `osConfLib.dll`, addressing each setting by name (`Enable_EQ_GFX`, `EQ_Slider1` …
`EQ_Slider10`, `AUDIOBRILLIANT_LEVEL`, and so on). Those names were read out of the library's
own string table. The registry write remains as a fallback when the library is not present.

Battery level and sound mode are not audio properties at all — they are vendor HID reports on
usage page `0xFF90`, exchanged with the USB dongle (`src/Device/`).

## Requirements

- Windows 10/11 x64
- .NET 10 SDK to build
- Administrator rights at runtime, because `FxProperties` is only writable by elevated
  processes (the Hub elevates for the same reason)
- `osConfLib.dll` for live apply. The app looks for it next to the executable first, then in
  `C:\Program Files\MCHOSE HUB\libs`. Copy it next to the executable if you want to uninstall
  the Hub. Without it, settings are still saved but only take effect after replugging the
  headset.

## Drivers

The headset needs the C-Media CM2025 audio driver. The app checks for it at startup and, if
none of the known variants is present, offers to install it — nothing is installed without
your confirmation. It adds the base driver, the extension and the APO in that order and tells
you if Windows wants a reboot.

Driver files are taken from an installed MCHOSE HUB (`libs/<model>`), or from a `drivers/`
folder in the project if you have one. `drivers/` is not tracked here: those are C-Media and
MCHOSE signed binaries that are not mine to redistribute.

The same hardware ships under more than one package: a machine may be running
`mchose_v9_pro.inf` or the generic C-Media `blk2025ghs.inf`, and both work. The app treats any
known variant as installed rather than forcing the MCHOSE-branded one over a working driver.

Two things to expect after a fresh install. Windows does not create the audio endpoint until
the device re-enumerates, so replug the dongle or reboot if effects stay unavailable — the app
retries on its own once the endpoint appears. And a new install recreates the effect settings
from defaults, so your equalizer curve will be flat again.

There is no standalone driver download to point at. MCHOSE's support pages say the previous
V9/V9 Pro driver is no longer offered, and the model has been dropped from their live update
manifest — `cdn.mchose.com.cn/OTA/PC/prod/otaFile/otaConfigApp_prod.json` lists V9 TURBO,
V9 TURBO+ and V9 PRO 2 but no V9 PRO, and the only driver installers hosted there are THX APO
packages for other models. The driver now ships solely inside the HUB installer.

## Build and run

```
dotnet build V9ProControl.csproj
```

The executable lands in `bin/Debug/net10.0-windows/` and prompts for elevation when started.

## Repository layout

```
src/Apo/        audio processing object properties: equalizer, effects, osConfLib interop
src/Audio/      Core Audio endpoint discovery, volume and mute
src/Device/     HID transport, battery, sound mode, driver detection and install
src/Ui/         dark theme and the equalizer graph control
MainForm.cs     the single window
```

## Notes and limits

- The device's own microphone-mute button produces no USB traffic at all, so it cannot be
  read or triggered from the host. The app's mute is the Windows endpoint mute, which is what
  the Hub's toggle does too.
- Driver packages are not included in this repository; they belong to C-Media/MCHOSE. See
  [Drivers](#drivers) for how to restore them.
- `osConfLib.dll` writes a debug log into the working directory of whatever process loads it,
  using an uninitialised filename. It is harmless, but that is where those files come from.
- Band centre frequencies are fixed in the APO's DSP. Only the ten gains are adjustable.
