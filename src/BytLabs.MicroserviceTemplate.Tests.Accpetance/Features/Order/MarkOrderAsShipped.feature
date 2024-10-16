Feature: Mark Order As Shipped

  As a user, I want to mark an order as shipped using a GraphQL mutation, ensuring the status is updated correctly for valid inputs and invalid inputs result in appropriate errors.

  @Order @Ignore
  Scenario: Successfully mark an order as shipped
    Given a valid `markOrderAsShipped` input
    When the `markOrderAsShipped` GraphQL mutation is called
    Then the order status should be updated to "Shipped"

  @Order @Ignore
  Scenario: Fail to mark an order as shipped with invalid input
    Given an invalid `markOrderAsShipped` input
    When the `markOrderAsShipped` GraphQL mutation is called
    Then the order status should not be updated
    And an appropriate error message should be returned
