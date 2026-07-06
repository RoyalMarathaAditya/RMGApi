-- ============================================================
-- Script: UpdateProjectCSM.sql
-- Description: Populate CSM column in Projects table based on
--              ProjectCode mapping. NULL values are stored where
--              mapping indicates NULL.
-- ============================================================

-- First clear existing CSM values to start fresh
UPDATE Projects SET CSM = NULL WHERE CSM IS NOT NULL;

-- Apply mapping
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000102';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000103';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000110';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000112';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000133';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000165';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000167';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000168';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000174';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000175';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000176';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000177';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000182';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000184';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000185';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000186';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000187';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000188';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000190';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000191';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000193';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000194';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000195';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000196';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000197';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000199';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000201';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000202';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000203';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000204';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000205';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000206';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000207';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000208';
UPDATE Projects SET CSM = 'Balan Ramaswamy'   WHERE ProjectCode = 'NV000214';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000219';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000220';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000223';
UPDATE Projects SET CSM = 'Kapil Godani'      WHERE ProjectCode = 'NV000238';
UPDATE Projects SET CSM = 'Smita Bhatia'      WHERE ProjectCode = 'NV000240';
UPDATE Projects SET CSM = NULL                WHERE ProjectCode = 'NV000242';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000252';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000257';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000259';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000261';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000264';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000265';
UPDATE Projects SET CSM = 'Kapil Godani'      WHERE ProjectCode = 'NV000271';
UPDATE Projects SET CSM = 'Kapil Godani'      WHERE ProjectCode = 'NV000276';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000278';
UPDATE Projects SET CSM = 'Kapil Godani'      WHERE ProjectCode = 'NV000280';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000281';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000282';
UPDATE Projects SET CSM = 'Kapil Godani'      WHERE ProjectCode = 'NV000283';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000284';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000285';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000286';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000288';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000289';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000290';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000291';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000292';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000293';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000294';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000295';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000296';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000297';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000298';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000299';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000300';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000302';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000303';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000305';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000306';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000307';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000310';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000311';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000312';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000313';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000314';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000315';
UPDATE Projects SET CSM = 'Ravindra Bhuyarkar' WHERE ProjectCode = 'NV000317';
UPDATE Projects SET CSM = 'Harshad Mandlecha' WHERE ProjectCode = 'NV000320';
UPDATE Projects SET CSM = 'Kapil Godani'      WHERE ProjectCode = 'NV000321';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000322';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000324';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000325';
UPDATE Projects SET CSM = 'Saksham Sarode'    WHERE ProjectCode = 'NV000326';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000329';
UPDATE Projects SET CSM = 'Ravindra Bhuyarkar' WHERE ProjectCode = 'NV000330';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000331';
UPDATE Projects SET CSM = 'Lawlesh Tiwari'    WHERE ProjectCode = 'NV000334';

-- Log ProjectCodes from the mapping that do NOT exist in the Projects table
SELECT 'SKIPPED - ProjectCode not found in DB: ' + Code AS LogMessage
FROM (VALUES
  ('NV000102'),('NV000103'),('NV000110'),('NV000112'),('NV000133'),('NV000165'),
  ('NV000167'),('NV000168'),('NV000174'),('NV000175'),('NV000176'),('NV000177'),
  ('NV000182'),('NV000184'),('NV000185'),('NV000186'),('NV000187'),('NV000188'),
  ('NV000190'),('NV000191'),('NV000193'),('NV000194'),('NV000195'),('NV000196'),
  ('NV000197'),('NV000199'),('NV000201'),('NV000202'),('NV000203'),('NV000204'),
  ('NV000205'),('NV000206'),('NV000207'),('NV000208'),('NV000214'),('NV000219'),
  ('NV000220'),('NV000223'),('NV000238'),('NV000240'),('NV000242'),('NV000252'),
  ('NV000257'),('NV000259'),('NV000261'),('NV000264'),('NV000265'),('NV000271'),
  ('NV000276'),('NV000278'),('NV000280'),('NV000281'),('NV000282'),('NV000283'),
  ('NV000284'),('NV000285'),('NV000286'),('NV000288'),('NV000289'),('NV000290'),
  ('NV000291'),('NV000292'),('NV000293'),('NV000294'),('NV000295'),('NV000296'),
  ('NV000297'),('NV000298'),('NV000299'),('NV000300'),('NV000302'),('NV000303'),
  ('NV000305'),('NV000306'),('NV000307'),('NV000310'),('NV000311'),('NV000312'),
  ('NV000313'),('NV000314'),('NV000315'),('NV000317'),('NV000320'),('NV000321'),
  ('NV000322'),('NV000324'),('NV000325'),('NV000326'),('NV000329'),('NV000330'),
  ('NV000331'),('NV000334')
) AS Mapping(Code)
WHERE NOT EXISTS (SELECT 1 FROM Projects WHERE ProjectCode = Mapping.Code);
GO
