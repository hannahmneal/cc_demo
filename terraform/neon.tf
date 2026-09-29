# This is a community-maintained provider, not an official Neon product.
resource "neon_project" "cc_demo" {
  name       = "cc-demo"
  org_id     = var.neon_org_id
  region_id  = "aws-us-east-2"
  pg_version = 17

  # Free plan caps point-in-time-restore history at 6h (21600s); the
  # provider's own default (86400s/24h) exceeds that and gets rejected.
  history_retention_seconds = 21600
}

locals {
  # Npgsql does NOT support Neon's postgres:// URI format (connection_uri /
  # connection_uri_pooler) - it only parses keyword=value connection strings,
  # so build one from the individual attributes instead.
  pg_connection_string = join(";", [
    "Host=${neon_project.cc_demo.database_host_pooler}",
    "Port=5432",
    "Database=${neon_project.cc_demo.database_name}",
    "Username=${neon_project.cc_demo.database_user}",
    "Password=${neon_project.cc_demo.database_password}",
    "SSL Mode=Require",
  ])
}

output "neon_connection_uri" {
  value     = local.pg_connection_string
  sensitive = true
}
