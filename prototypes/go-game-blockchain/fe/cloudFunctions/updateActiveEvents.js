// Cannot be detected by moralis watch-cloud-file for .ts files
// cli: yarn hardhat:cloud
import Moralis from "moralis/types";

Moralis.Cloud.afterSave("FindMatch", async (request) => {
  const confirmed = request.object.get("confirmed");
  const logger = Moralis.Cloud.getLogger();
  logger.info("Looking for confirmed tx");
  if (confirmed) {
    console.log("confirmed");
  }
});
