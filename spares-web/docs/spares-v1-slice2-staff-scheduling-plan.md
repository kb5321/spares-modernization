SpaRes V1 — Slice 2 — Step 2L: Delete Daily Schedule

Status: COMPLETE — November 15, 2026. Functionally complete; Git commit/push is pending.

Read docs/spares-v1-slice2-staff-scheduling-plan.md.

Inspect:
- spares-web/src/components/StaffScheduleManager.tsx
- SpaRes.Api StaffScheduleController and relevant reservation validation code

Objective: Add Delete Daily Schedule to the existing React scheduling manager.

Requirements:
1. Show Delete Schedule only when an existing schedule is selected.
2. Require explicit user confirmation before deletion.
3. Use DELETE /api/StaffSchedule/{id}.
4. Handle HTTP 204 No Content without parsing JSON.
5. After successful deletion, clear the schedule state, reset the form, and display a success message.
6. Prevent duplicate delete requests and disable conflicting actions while deleting.
7. Preserve existing GET, POST, and PUT behavior.
8. Block deletion with HTTP 409 Conflict only when the selected schedule has matching Pending or Confirmed reservations for its StaffId, LocationId, and Date. Cancelled and Void reservations do not block deletion; there is no date cutoff.
9. Keep changes minimal and scoped to Step 2L.

Completion record covers:
- Current code findings and implementation
- Files changed, risks, and validation
- Approval decisions and future offboarding constraints

## Completion Record — November 15, 2026

- The scheduling manager now offers confirmed, date-specific deletion for an existing schedule. It sends `DELETE /api/StaffSchedule/{id}`, handles 204 without parsing a body, prevents duplicate/conflicting actions, resets the schedule form after success, and shows a result message. Existing GET, POST, and PUT behavior is preserved.
- The backend protects matching Pending and Confirmed reservations with HTTP 409 Conflict, regardless of schedule date. Cancelled and Void reservations do not block deletion. Deleting a schedule does not delete reservation records or history.
- The retained ID-specific DELETE action has `[Authorize]`. The unused and unsafe `DELETE /api/StaffSchedule/staff/{staffId}` action was removed.
- Frontend and backend builds passed. Nine browser/Swagger checks passed manually; these were not automated tests. The full ESLint run still reports existing violations; the focused component lint passed with the pre-existing `react-hooks/set-state-in-effect` rule suppressed.
- Step 2L is functionally complete. Other Slice 2 tasks remain open. Git commit/push is pending.
- Future staff-offboarding rule: reschedule a departing therapist's future Pending or Confirmed appointments with different qualified therapists before removing future schedules. Preserve historical records. Offboarding was not implemented in Step 2L.