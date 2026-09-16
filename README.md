# CC_Demo

A fun little project I'm working on.

Live at: https://cc-demo.redqueen.run


## TLDR; Deployment Architecture (Backend)

A Microservice API hosted on Render with Cloudflare sitting in front of it as a DNS + proxy + security layer. Once a browser/client request reaches the container, the API code connects over the network to read/write data to the serverless database on Neon. 

Terraform scaffolds the infrastructure. In order to avoid tf state collisions on successive CI runs, Cloudflare's R2 Object Storage bucket is used for shared state storage.

GitHub Actions provides easy credential delivery and deployment: pushes to `master` sets the deployment into motion.

### 🚩 Gotchas 🚩

- The services for this project were chosen specifically for their free tiers. This is why there aren't separate deployment environments (dev/qa/prod) for this project. If you want me to implement more environments, hire me or give me money.
- The `render_web_services.cc_demo_api` is pinned to the `:latest` tag because a known bug in the Render Terraform provider breaks *updates* to the free-tier service being used. Therefore, a Render Deploy Hook (webhook) tells Render to "go pull latest again", sidestepping the bug entirely. For more information, please see https://github.com/render-oss/terraform-provider-render/issues/80 or the comment in `terraform/render.tf`.
- Neon's free tier caps PITR history at 6h, forcing `history_retention_seconds = 21600` instead of the provider default (24h).
- Npgsql cannot parse Neon's `postgres://` URI format; therefore, the connection string is rebuilt from individual Neon attributes (i.e., `Host=...;Port=...;...`) in `neon.tf`.


