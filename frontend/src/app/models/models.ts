export interface Job {
  id?: number;
  title: string;
  organization?: string;
  location?: string;
  workplace?: string;
  url?: string;
  source?: string;
  status: string;
  priority: string;
  salary?: string;
  dateFound?: string;
  dateApplied?: string;
  nextAction?: string;
  nextActionDate?: string;
  notes?: string;
  description?: string;
  externalId?: string;
  jobType?: string;
  schedule?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface JobListing {
  title: string;
  organization?: string;
  location?: string;
  workplace?: string;
  url?: string;
  source?: string;
  siteId?: string;
  description?: string;
  salaryText?: string;
  salaryMin?: number;
  salaryMax?: number;
  salaryCurrency?: string;
  salaryPeriod?: string;
  jobType?: string;
  schedule?: string;
  industry?: string;
  tags: string[];
  published?: string;
  externalId?: string;
  score: number;
}

export interface Contact {
  id?: number;
  name: string;
  organization?: string;
  title?: string;
  email?: string;
  linkedIn?: string;
  phone?: string;
  relationship?: string;
  notes?: string;
}

export interface Outreach {
  id?: number;
  contactName?: string;
  organization?: string;
  jobTitle?: string;
  channel: string;
  subject?: string;
  activityDate: string;
  followUpDate?: string;
  outcome?: string;
  notes?: string;
}

export interface Organization {
  id?: number;
  name: string;
  website?: string;
  industry?: string;
  location?: string;
  notes?: string;
}

export interface SiteConfig {
  id: string;
  provider: string;
  name: string;
  enabled: boolean;
  maxResults: number;
  options: Record<string, any>;
  notes: string;
}

export interface AppConfig {
  version: number;
  updatedAt?: string;
  sites: SiteConfig[];
  preferences: {
    titles: string[];
    skills: string[];
    industries: string[];
    locations: string[];
    workplaces: string[];
    jobTypes: string[];
    salary: {
      minimum?: number;
      maximum?: number;
      currency: string;
      period: string;
      includeUnlisted: boolean;
    };
    maxAgeDays?: number;
  };
  search: {
    strictFiltering: boolean;
    dedupeByUrl: boolean;
    timeoutSeconds: number;
    userAgent: string;
  };
}

export interface DashboardStats {
  totalJobs: number;
  byStatus: { status: string; count: number }[];
  contacts: number;
  outreach: number;
  overdueFollowUps: number;
}
