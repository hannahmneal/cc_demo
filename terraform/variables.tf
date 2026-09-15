variable "render_api_key" {
  description = "Render API key (Account Settings > API Keys)"
  type        = string
  sensitive   = true
}

variable "render_owner_id" {
  description = "Render workspace/owner ID that will own the web service"
  type        = string
}

variable "neon_api_key" {
  description = "Neon API key (Account Settings > API Keys)"
  type        = string
  sensitive   = true
}

variable "neon_org_id" {
  description = "The id of the organization of the Neon project"
  type        = string
}

variable "cloudflare_api_token" {
  description = "Cloudflare API token scoped to Zone:DNS:Edit and Zone:Zone:Read for redqueen.run"
  type        = string
  sensitive   = true
}

variable "cloudflare_zone_id" {
  description = "Zone ID for redqueen.run in Cloudflare"
  type        = string
}

variable "image_repository" {
  description = "GHCR image reference to deploy, e.g. ghcr.io/hannahmneal/cc_demo"
  type        = string
}

variable "image_tag" {
  description = "Image tag to deploy (set per CI run, e.g. the git SHA)"
  type        = string
  default     = "latest"
}

variable "subdomain" {
  description = "Single-level subdomain under redqueen.run for this project"
  type        = string
  default     = "cc-demo"
}
