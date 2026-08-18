# Meeting Assistant Local Setup

Last verified: 2026-07-10

This document records the Meeting Assistant setup on Daniel's macOS machine. The source repository lives inside DanielsVault, but the running binaries and launch scripts live under `~/Library/Application Support` so macOS `launchd` can start them without Documents privacy restrictions.

## Source Checkout

- Repository: `/Users/dh/Documents/DanielsVault/_ops/meeting-assistant`
- Remote: `https://gitea.schweigert.cloud/Manuel/meeting-assistant`
- Main branch: `main`
- Vault used by the assistant: `/Users/dh/Documents/DanielsVault`

The checkout has local ignored files:

- `.env`
- `tmp/run-local.sh`
- `.git/hooks/post-merge`
- `.git/hooks/post-rewrite`

The hooks are local Git metadata and are not versioned with the repository.

## Installed Runtime

The LaunchAgent runs a published portable `net10.0` build from:

- App directory: `/Users/dh/Library/Application Support/MeetingAssistant/app`
- Staging directory used during updates: `/Users/dh/Library/Application Support/MeetingAssistant/app-staging`
- Runner: `/Users/dh/Library/Application Support/MeetingAssistant/bin/run-meeting-assistant.sh`
- Updater: `/Users/dh/Library/Application Support/MeetingAssistant/bin/update-from-repo.sh`
- Runtime config: `/Users/dh/Library/Application Support/MeetingAssistant/config/meeting-assistant.env`

The runner executes:

```bash
/usr/local/share/dotnet/dotnet "/Users/dh/Library/Application Support/MeetingAssistant/app/MeetingAssistant.dll"
```

with `--contentRoot` set to the installed app directory and `--urls` defaulting to `http://localhost:5090`.

## LaunchAgent

The service is installed as a user LaunchAgent:

- Label: `cloud.schweigert.meeting-assistant`
- Plist: `/Users/dh/Library/LaunchAgents/cloud.schweigert.meeting-assistant.plist`
- Runs at user-session startup via `RunAtLoad`
- Restarts automatically via `KeepAlive`

Important commands:

```bash
launchctl print gui/$(id -u)/cloud.schweigert.meeting-assistant
launchctl kickstart -k gui/$(id -u)/cloud.schweigert.meeting-assistant
launchctl bootout gui/$(id -u) ~/Library/LaunchAgents/cloud.schweigert.meeting-assistant.plist
launchctl bootstrap gui/$(id -u) ~/Library/LaunchAgents/cloud.schweigert.meeting-assistant.plist
```

## Configuration

Runtime configuration is in:

```text
/Users/dh/Library/Application Support/MeetingAssistant/config/meeting-assistant.env
```

Key settings:

- `DANIELSVAULT_PATH=/Users/dh/Documents/DanielsVault`
- `ASPNETCORE_URLS=http://localhost:5090`
- `MeetingAssistant__Vault__BaseFolder=$DANIELSVAULT_PATH`
- Meeting artifacts are written below the vault:
  - `Meetings/Transcripts`
  - `Meetings/Notes`
  - `Meetings/Assistant Context`
  - `Meetings/Summaries`
  - `Projects`
  - `Meetings/Dictionary.md`

Local runtime state is outside the vault:

- Recordings: `~/Library/Application Support/MeetingAssistant/Recordings`
- Speaker DB: `~/Library/Application Support/MeetingAssistant/SpeakerIdentity/speaker-identities.db`
- FunASR model cache: `~/Library/Application Support/MeetingAssistant/FunASR/models`
- Pyannote model cache: `~/Library/Application Support/MeetingAssistant/Pyannote/models`

The current macOS setup disables Windows/live-heavy helpers in the launchd config:

- Calendar recording prompts
- Speaker identification
- Screenshot OCR
- Managed FunASR backend
- FunASR diarization
- Whisper local diarization

## Pull-To-Update Flow

The checkout has local Git hooks:

- `.git/hooks/post-merge`
- `.git/hooks/post-rewrite`

After a successful `git pull` that actually fast-forwards or merges new commits, `post-merge` runs:

```bash
~/Library/Application Support/MeetingAssistant/bin/update-from-repo.sh
```

For `git pull --rebase`, `post-rewrite` runs the same updater when Git reports a rebase.

The updater:

1. Checks `http://127.0.0.1:5090/recording/status`.
2. Skips publish/restart if Meeting Assistant is recording or in a non-idle state.
3. Publishes the current checkout:

   ```bash
   dotnet publish /Users/dh/Documents/DanielsVault/_ops/meeting-assistant/MeetingAssistant/MeetingAssistant.csproj \
     --framework net10.0 \
     --configuration Release \
     -p:EnableWindowsTargeting=true \
     -o "$HOME/Library/Application Support/MeetingAssistant/app-staging"
   ```

4. Syncs staged files into the installed app directory with `rsync --delete`.
5. Restarts the LaunchAgent with `launchctl kickstart -k`.
6. Waits for `/health` to pass.

To force an update even if status looks busy:

```bash
MEETING_ASSISTANT_FORCE_RESTART=1 ~/Library/Application\ Support/MeetingAssistant/bin/update-from-repo.sh
```

Use force only when you are sure no active meeting work will be interrupted.

## Health Checks

```bash
curl -fsS http://127.0.0.1:5090/health
curl -fsS http://127.0.0.1:5090/recording/status
lsof -nP -iTCP:5090 -sTCP:LISTEN
```

Expected idle status:

```json
{
  "isRecording": false,
  "transcriptPath": null,
  "meetingNotePath": null,
  "assistantContextPath": null,
  "summaryPath": null,
  "state": 0,
  "launchProfile": null
}
```

## Logs

- App stdout: `/Users/dh/Library/Logs/MeetingAssistant/meeting-assistant.out.log`
- App stderr: `/Users/dh/Library/Logs/MeetingAssistant/meeting-assistant.err.log`
- Pull/update log: `/Users/dh/Library/Logs/MeetingAssistant/update-from-repo.log`

Useful log commands:

```bash
tail -n 100 ~/Library/Logs/MeetingAssistant/meeting-assistant.out.log
tail -n 100 ~/Library/Logs/MeetingAssistant/meeting-assistant.err.log
tail -n 100 ~/Library/Logs/MeetingAssistant/update-from-repo.log
```

## Manual Update

Run:

```bash
~/Library/Application\ Support/MeetingAssistant/bin/update-from-repo.sh
```

This uses the same safety check, publish, install, restart, and health-check flow as the Git hooks.

## Notes And Caveats

- This is a user LaunchAgent, so it starts when the `dh` user session starts, not before login.
- The installed runtime is the portable `net10.0` target. The full Windows target cannot be built directly on this macOS machine because it tries to run Windows SDK tooling such as `MakePri.exe`.
- Running directly from `/Users/dh/Documents/DanielsVault/_ops/meeting-assistant` worked interactively, but launchd could not execute from the Documents-backed path due to macOS privacy restrictions. Publishing into `~/Library/Application Support/MeetingAssistant/app` avoids that.
- If `git pull` says `Already up to date.`, Git does not run `post-merge`, so the updater does not run. Use the manual update command if you want to republish without new commits.
