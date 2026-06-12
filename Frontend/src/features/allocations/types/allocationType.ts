export enum AllocationType {
  Billable = 1,
  NonBillable = 2,
  Internal = 3,
  Shadow = 4,
  Training = 5,
  Support = 6,
}

export const allocationTypeLabelMap: Record<AllocationType, string> = {
  [AllocationType.Billable]: 'Billable',
  [AllocationType.NonBillable]: 'Non-billable',
  [AllocationType.Internal]: 'Internal',
  [AllocationType.Shadow]: 'Shadow',
  [AllocationType.Training]: 'Training',
  [AllocationType.Support]: 'Support',
};

export const allocationTypeOptions = [
  { value: AllocationType.Billable, label: allocationTypeLabelMap[AllocationType.Billable] },
  { value: AllocationType.NonBillable, label: allocationTypeLabelMap[AllocationType.NonBillable] },
  { value: AllocationType.Internal, label: allocationTypeLabelMap[AllocationType.Internal] },
  { value: AllocationType.Shadow, label: allocationTypeLabelMap[AllocationType.Shadow] },
  { value: AllocationType.Training, label: allocationTypeLabelMap[AllocationType.Training] },
  { value: AllocationType.Support, label: allocationTypeLabelMap[AllocationType.Support] },
];
