# This is a community-maintained provider, not an official Neon product.
# Double-check resource/attribute names against the current provider docs
# (https://registry.terraform.io/providers/kislerdm/neon/latest/docs) before
# your first `terraform apply`, since they can change between versions.
resource "neon_project" "cc_demo" {
  name       = "cc-demo"
  org_id     = var.neon_org_id
  region_id  = "aws-us-east-2"
  pg_version = 17

  # Free plan caps point-in-time-restore history at 6h (21600s); the
  # provider's own default (86400s/24h) exceeds that and gets rejected.
  history_retention_seconds = 21600
}

output "neon_connection_uri" {
  value     = neon_project.cc_demo.connection_uri_pooler
  sensitive = true
}
