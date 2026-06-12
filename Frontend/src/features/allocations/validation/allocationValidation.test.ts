import { allocationValidationSchema } from './allocationValidation';

describe('allocation validation schema', () => {
  it('validates a complete allocation payload', async () => {
    const payload = {
      employeeId: 1,
      projectId: 2,
      allocationPercentage: 60,
      allocationType: 1,
      startDate: '2026-06-02',
      endDate: '2026-07-02',
      isActive: true,
    };

    await expect(allocationValidationSchema.validate(payload)).resolves.toBe(payload);
  });

  it('rejects allocation percentage beyond 100', async () => {
    const payload = {
      employeeId: 1,
      projectId: 2,
      allocationPercentage: 120,
      allocationType: 1,
      startDate: '2026-06-02',
      endDate: '2026-07-02',
      isActive: true,
    };

    await expect(allocationValidationSchema.validate(payload)).rejects.toThrow(/cannot exceed 100/);
  });

  it('rejects end date before start date', async () => {
    const payload = {
      employeeId: 1,
      projectId: 2,
      allocationPercentage: 50,
      allocationType: 1,
      startDate: '2026-06-10',
      endDate: '2026-06-05',
      isActive: true,
    };

    await expect(allocationValidationSchema.validate(payload)).rejects.toThrow(/End date must be after start date/);
  });
});
