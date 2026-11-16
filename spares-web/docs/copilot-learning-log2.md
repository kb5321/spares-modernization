# SpaRes Modernization — GitHub Copilot Learning Log

**Project:** SpaRes V1 — Slice 2: Staff Scheduling Manager  
**Purpose:** Preserve the instructions given to Copilot, its proposals and responses, human decisions, implementation reviews, test results, and learning checkpoints.  
**Last updated:** November 15, 2026  
**Document type:** Historical learning journal (append new entries; do not overwrite earlier sessions)

> **Documentation rule:** The implementation plan is a *living document*. This learning log is a *historical record*. Keep original prompts and Copilot responses verbatim when available. Label summaries and reconstructions clearly; never invent missing transcripts.

## 1. Project context

- **Backend:** ASP.NET Core Web API, EF Core, SQLite.
- **Frontend:** React, TypeScript, Vite.
- **Completed V1 slice:** Reservation Manager; preserve its working behavior.
- **Current slice:** Staff Scheduling Manager.
- **Scheduling model:** One date-specific working shift per staff member, location, and date; multiple dated breaks; single-day or multi-day absences.
- **Key frontend file:** `spares-web/src/components/StaffScheduleManager.tsx`.
- **Working implementation plan:** `docs/spares-v1-slice2-staff-scheduling-plan.md`.

## 2. How to record each Copilot session

For every new task, append an entry with these fields:

1. **Date, step, and objective**
2. **Original prompt/plan given to Copilot** — exact text when saved; otherwise explicitly mark as a reconstruction
3. **Copilot's proposed plan** — exact response if available, including alternatives or risks
4. **Human review and approval** — what was accepted, rejected, or changed
5. **Copilot's implementation response** — exact text or an explicitly labeled summary
6. **Files changed and code review** — key decisions and explanations
7. **Build and browser tests** — distinguish tool-reported results from independently tested results
8. **Lessons learned and next step**

Do not rewrite an old session when a new one starts. Append new sessions in chronological order.

---

## 3. Session — Step 2K: Edit Existing Daily Schedule

**Date:** November 15, 2026  
**Status:** COMPLETE — PASS  
**Implementation:** GitHub Copilot Agent in VS Code  
**Scope:** Frontend only; `StaffScheduleManager.tsx`

### 3.1 Objective and starting point

The Staff Scheduling Manager could already select staff, location, and date; retrieve and display an existing daily schedule; and create a schedule with POST. The next task was to **edit an existing daily schedule without breaking creation or retrieval**.

### 3.2 Plan/instructions given to Copilot

**Source status:** The following requirements were preserved in the working plan shared in our discussion. They are **not claimed to be a verbatim copy of the full Copilot Chat prompt**.

1. Inspect existing frontend and backend code.
2. Populate time inputs from the selected schedule.
3. Use the existing PUT endpoint.
4. Send times in `HH:mm:ss` format.
5. Validate that End Time is after Start Time.
6. Refresh the displayed schedule after saving.
7. Preserve existing functionality.
8. Do not modify the backend or database without an approved reason.

**Workflow constraints:** Inspect and propose a plan first; review and approve before editing; limit edits to the agreed frontend file; run the frontend build; test in the browser; review the diff before accepting; do not commit or push.

**Original verbatim prompt:** Not available in this record. Paste the original prompt here later if it is retained in Copilot Chat.

### 3.3 Copilot's proposed plan

A read-only Copilot session produced a plan before editing. The user then opened a new Agent session and authorized changes to `StaffScheduleManager.tsx` only.

**Exact proposed-plan text:** Not available in this record. Do not treat the implementation summary below as the earlier proposal.

### 3.4 Copilot's implementation response

**Recorded Copilot response (as reported in our discussion):**

> Completed 15 steps in 1m 1s. +47 / -12.  
> Implemented Step 2K in `StaffScheduleManager.tsx`.  
> Existing schedules populate time inputs in `HH:mm` format and can be edited with `PUT /api/StaffSchedule/{id}`.  
> Times are sent as `HH:mm:ss`. A successful 204 is handled without attempting to parse a response body; the displayed schedule updates after saving.  
> New schedules still use POST, and schedule retrieval still uses GET.  
> A ref guard and disabled controls prevent duplicate submissions.  
> Validation: `npm --prefix spares-web run build` passed.  
> I did not commit. The worktree also contains other changed and untracked files; I left them untouched.

**Reported diff:** One file changed, 47 insertions and 12 deletions.  
**Reported build result:** PASS (Copilot-reported; not independently rerun in this record).  
**Git:** No commit or push as part of this step.

### 3.5 Human code review and React learning

#### A. Guarding against duplicate saves

Reviewed code:

```tsx
const [isSaving, setIsSaving] = useState(false)
const isSavingRef = useRef(false)
```

```tsx
if (
    !selectedStaffId ||
    !selectedLocationId ||
    !selectedDate ||
    isSavingRef.current
) {
    return
}

isSavingRef.current = true
setIsSaving(true)
```

**Lesson:** `useState` updates the UI when its value changes. `useRef` holds a mutable value across renders without triggering a render. The ref acts as an immediate duplicate-request guard; the state drives disabled controls and the “Saving...” label. The guard must be reset when saving finishes, including on error. **The exact cleanup block was not pasted into our review**, so this remains a code-inspection checkpoint rather than a separately verified line-by-line result.

#### B. Loading existing schedule hours

Reviewed code:

```tsx
setStartTime(matchingSchedule?.startTime.substring(0, 5) ?? '09:00')
setEndTime(matchingSchedule?.endTime.substring(0, 5) ?? '18:00')
```

**Lessons:**

- `?.` safely handles a missing schedule.
- `??` supplies a fallback when the left expression is null or undefined.
- `.substring(0, 5)` converts an API time such as `09:00:00` to the HTML time-input format `09:00`.
- When no schedule is selected, default times are restored instead of retaining a previous schedule's values.

#### C. Choosing create versus edit

Reviewed code:

```tsx
const isEditing = schedule !== null
```

```tsx
isEditing
    ? `https://localhost:7021/api/StaffSchedule/${schedule.staffScheduleId}`
    : 'https://localhost:7021/api/StaffSchedule'
```

```tsx
method: isEditing ? 'PUT' : 'POST',
```

**Lesson:** A single boolean controls both the endpoint and HTTP method. `POST` creates a new schedule; `PUT /api/StaffSchedule/{id}` updates the existing one. The ternary operator (`condition ? trueValue : falseValue`) provides a compact conditional expression.

#### D. Reusing the request body

Reviewed code (unchanged from creation flow):

```tsx
body: JSON.stringify({
    staffId: Number(selectedStaffId),
    locationId: Number(selectedLocationId),
    date: selectedDate,
    startTime: `${startTime}:00`,
    endTime: `${endTime}:00`
})
```

**Lesson:** `Number(...)` converts selector values to numeric IDs; appending `:00` produces `HH:mm:ss`, matching the format required by this API. Reusing the payload reduces duplication.

#### E. Updating the display and success message

Reviewed code:

```tsx
setSchedule(savedSchedule)
setScheduleMessage(
    isEditing
        ? 'Schedule updated successfully.'
        : 'Schedule created successfully.'
)
```

**Lesson:** One save function can serve two operations and show the correct message. Copilot reported handling HTTP 204 No Content without parsing an empty JSON body; the complete code that constructs `savedSchedule` was **not pasted into our line-by-line review**.

#### F. One form, two modes

Reviewed behavior and JSX:

```tsx
<h3>{schedule ? 'Edit Daily Schedule' : 'New Daily Schedule'}</h3>
```

```tsx
<button type="button" disabled={isSaving} onClick={saveSchedule}>
    {isSaving
        ? 'Saving...'
        : schedule
          ? 'Update Schedule'
          : 'Save Schedule'}
</button>
```

The time inputs are controlled by `startTime` and `endTime` state and disabled while saving. The form appears only when staff, location, and date are selected.

**Lesson:** One React component and one button can support multiple user-visible behaviors, driven by state. This is declarative rendering, not two separate forms.

### 3.6 Test and acceptance record

| Check | Result | Evidence |
|---|---|---|
| Frontend build | PASS | Reported by Copilot |
| Existing schedule loads into time inputs | PASS | User-reviewed UI and code |
| Update through PUT | PASS | User browser test |
| Updated hours persist after refresh | PASS | User browser test |
| Shared Save/Update button labels | PASS | User browser observation |
| Copilot diff accepted | PASS | User clicked **Keep** |
| Local file saved | PASS | User confirmed in VS Code |
| Git commit/push | NOT DONE | Intentionally deferred |

**Acceptance decision:** Step 2K COMPLETE — PASS. Accepting the diff with **Keep** does not itself commit or push the changes to GitHub.

### 3.7 Engineering lessons

- Review AI-generated code rather than assuming a successful build proves correct behavior.
- Use narrowly scoped Copilot Agent instructions and restrict changed files.
- Compare the actual implementation against the original plan.
- Distinguish **Copilot-reported build PASS** from **human-verified browser PASS**.
- Preserve existing GET/POST behavior when adding PUT.
- Prefer one reusable create/edit form when state cleanly determines behavior.
- **Working rule:** One change → One test → Confirm → Next.

### 3.8 Next step

**Step 2L — Delete Daily Schedule.** Before implementation, inspect the existing DELETE endpoint and define confirmation and failure behavior. Preserve working GET, POST, and PUT functionality. Record the next Copilot prompt, proposed plan, response, review, and test results in a **new section** of this log.

---

## 4. Template for future sessions

Copy this section and append it below previous sessions; never replace historical entries.

```markdown
## Session — Step [ID]: [Task]

**Date:** YYYY-MM-DD
**Status:** PLANNED / IN PROGRESS / COMPLETE — PASS / BLOCKED
**Scope:** [Allowed files]

### Original prompt/plan given to Copilot
[Paste exact text. If unavailable, explicitly label a reconstruction.]

### Copilot's proposed plan
[Paste exact response or mark unavailable.]

### Human approval/decisions
[What was approved, rejected, or revised?]

### Copilot's implementation response
[Paste exact response; note files and diff size.]

### Code review and lessons
[Key snippets, explanations, risks, questions.]

### Build and browser testing
[Separate Copilot-reported tests from user-confirmed tests.]

### Acceptance and next step
[Keep/Undo, commit status, next task.]
```
