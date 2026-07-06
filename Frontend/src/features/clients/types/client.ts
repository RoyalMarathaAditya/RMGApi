export interface Client {
  id: number;
  name: string;
  contractStartDate: string | null;
  contractEndDate: string | null;
  statusId: string;
  statusName: string;
  location: string | null;
  createdOn: string;
  createdBy: string | null;
  modifiedOn: string | null;
  modifiedBy: string | null;
  rowVersion: string;
}

export interface CreateClientRequest {
  name: string;
  contractStartDate: string | null;
  contractEndDate: string | null;
  statusId: string;
  location: string | null;
}

export interface UpdateClientRequest {
  name: string;
  contractStartDate: string | null;
  contractEndDate: string | null;
  statusId: string;
  location: string | null;
  rowVersion: string;
}

export interface MasterDto {
  id: string;
  name: string;
}
